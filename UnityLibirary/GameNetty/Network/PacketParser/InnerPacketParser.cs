#if GAMESERVER_NET
using System.Buffers;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace GameNetty;

/// <summary>
/// 用于处理内部网络数据包信息的类。
/// </summary>
public sealed class InnerPackInfo : APackInfo
{
    /// <summary>
    /// 创建一个 <see cref="InnerPackInfo"/> 实例，并将其与内存资源关联。
    /// </summary>
    /// <param name="memoryOwner">用于存储数据的内存资源。</param>
    /// <returns>创建的 <see cref="InnerPackInfo"/> 实例。</returns>
    public static InnerPackInfo Create(IMemoryOwner<byte> memoryOwner)
    {
        var innerPackInfo = Rent<InnerPackInfo>();
        innerPackInfo.MemoryOwner = memoryOwner;
        return innerPackInfo;
    }

    /// <summary>
    /// 创建一个 <see cref="MemoryStream"/> 实例，用于存储内存数据，并返回该实例。
    /// </summary>
    /// <returns>创建的 <see cref="MemoryStream"/> 实例。</returns>
    public override MemoryStream CreateMemoryStream()
    {
        // 创建可回收的内存流，用于存储消息数据
        var recyclableMemoryStream = MemoryStreamHelper.GetRecyclableMemoryStream();
        // 将内存资源中的消息数据写入内存流
        // 写入从内存起始位置到消息头长度+消息体长度的数据
        recyclableMemoryStream.Write(MemoryOwner.Memory.Span.Slice(0, Packet.InnerPacketHeadLength + MessagePacketLength));
        // 将内存流的指针定位到起始位置
        recyclableMemoryStream.Seek(0, SeekOrigin.Begin);

        // 返回创建的内存流
        return recyclableMemoryStream;
    }

    /// <summary>
    /// 释放当前 <see cref="InnerPackInfo"/> 实例及其关联的资源。
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        // 将当前的 InnerPackInfo 实例归还到对象池，以便重复利用
        Pool<InnerPackInfo>.Return(this);
    }

    /// <summary>
    /// 将消息数据从内存反序列化为指定的消息类型实例。
    /// </summary>
    /// <param name="messageType">目标消息类型。</param>
    /// <returns>反序列化后的消息类型实例。</returns>
    public override object Deserialize(Type messageType)
    {
        // 获取内存资源的引用
        var memoryOwnerMemory = MemoryOwner.Memory;
        // 获取消息体数据的切片
        memoryOwnerMemory = memoryOwnerMemory.Slice(Packet.InnerPacketHeadLength, MessagePacketLength);
        
        switch (ProtocolCode)
        {
            case >= Opcode.InnerBsonRouteResponse:
            {
                return MongoHelper.Instance.Deserialize(memoryOwnerMemory, messageType);
            }
            case >= Opcode.InnerRouteResponse:
            {
                return ProtoBufHelper.FromMemory(messageType, memoryOwnerMemory);
            }
            case >= Opcode.OuterRouteResponse:
            {
                return ProtoBufHelper.FromMemory(messageType, memoryOwnerMemory);
            }
            case >= Opcode.InnerBsonRouteMessage:
            {
                return MongoHelper.Instance.Deserialize(memoryOwnerMemory, messageType);
            }
            case >= Opcode.InnerRouteMessage:
            case >= Opcode.OuterRouteMessage:
            {
                return ProtoBufHelper.FromMemory(messageType, memoryOwnerMemory);
            }
            case >= Opcode.InnerBsonResponse:
            {
                return MongoHelper.Instance.Deserialize(memoryOwnerMemory, messageType);
            }
            case >= Opcode.InnerResponse:
            {
                return ProtoBufHelper.FromMemory(messageType, memoryOwnerMemory);
            }
            case >= Opcode.OuterResponse:
            {
                Log.Error($"protocolCode:{ProtocolCode} Does not support processing protocol");
                return null;
            }
            case >= Opcode.InnerBsonMessage:
            {
                return MongoHelper.Instance.Deserialize(memoryOwnerMemory, messageType);
            }
            case >= Opcode.InnerMessage:
            {
                return ProtoBufHelper.FromMemory(messageType, memoryOwnerMemory);
            }
            default:
            {
                Log.Error($"protocolCode:{ProtocolCode} Does not support processing protocol");
                return null;
            }
        }
    }
}

/// <summary>
/// 用于解析内部网络数据包的类。
/// </summary>
public sealed class InnerPacketParser : APacketParser
{
    private uint _rpcId;
    private long _routeId;
    private uint _protocolCode;
    private int _messagePacketLength;
    private bool _isUnPackHead = true;
    private readonly byte[] _messageHead = new byte[Packet.InnerPacketHeadLength];

    /// <summary>
    /// 初始化一个新的 <see cref="InnerPacketParser"/> 实例。
    /// </summary>
    public InnerPacketParser()
    {
        MemoryPool = MemoryPool<byte>.Shared;
    }

    /// <summary>
    /// 尝试解析循环缓冲区中的数据为一个数据包信息。
    /// </summary>
    /// <param name="buffer">待解析的循环缓冲区。</param>
    /// <param name="packInfo">解析后的数据包信息。</param>
    /// <returns>如果成功解析并获取数据包信息，则返回 true，否则返回 false。</returns>
    public override bool UnPack(CircularBuffer buffer, out APackInfo packInfo)
    {
        packInfo = null;

        // 在对象没有被释放的情况下循环解析数据
        while (!IsDisposed)
        {
            if (_isUnPackHead)
            {
                // 如果缓冲区中的数据长度小于内部消息头的长度，无法解析
                if (buffer.Length < Packet.InnerPacketHeadLength)
                {
                    return false;
                }

                // 从缓冲区中读取内部消息头的数据
                _ = buffer.Read(_messageHead, 0, Packet.InnerPacketHeadLength);
                _messagePacketLength = BitConverter.ToInt32(_messageHead, 0);

                // 检查消息体长度是否超出限制
                if (_messagePacketLength > Packet.PacketBodyMaxLength)
                {
                    throw new ScanException($"The received information exceeds the maximum limit = {_messagePacketLength}");
                }

                // 解析协议编号、RPC ID 和 Route ID
                _protocolCode = BitConverter.ToUInt32(_messageHead, Packet.PacketLength);
                _rpcId = BitConverter.ToUInt32(_messageHead, Packet.InnerPacketRpcIdLocation);
                _routeId = BitConverter.ToInt64(_messageHead, Packet.InnerPacketRouteRouteIdLocation);
                _isUnPackHead = false;
            }
    
            try
            {
                // 如果缓冲区中的数据长度小于消息体的长度，无法解析
                if (_messagePacketLength < 0 || buffer.Length < _messagePacketLength)
                {
                    return false;
                }
    
                _isUnPackHead = true;
                // 创建消息包
                var memoryOwner = MemoryPool.Rent(Packet.InnerPacketMaxLength);
                // 创建内部数据包信息实例
                packInfo = InnerPackInfo.Create(memoryOwner);
                // 设置数据包信息的属性值
                packInfo.RpcId = _rpcId;
                packInfo.RouteId = _routeId;
                packInfo.ProtocolCode = _protocolCode;
                packInfo.MessagePacketLength = _messagePacketLength;
                // 写入消息体的信息到内存中
                buffer.Read(memoryOwner.Memory.Slice(Packet.InnerPacketHeadLength), _messagePacketLength);
                // 写入消息头的信息到内存中
                _messageHead.AsMemory().CopyTo( memoryOwner.Memory.Slice(0, Packet.InnerPacketHeadLength));
                return true;
            }
            catch (Exception e)
            {
                // 在发生异常时，释放 packInfo 并记录日志
                packInfo?.Dispose();
                Log.Error(e);
                return false;
            }
        }
    
        return false;
    }

    /// <summary>
    /// 尝试从内存资源中解析数据为一个内部数据包信息。
    /// </summary>
    /// <param name="memoryOwner">包含数据的内存资源。</param>
    /// <param name="packInfo">解析后的内部数据包信息。</param>
    /// <returns>如果成功解析并获取内部数据包信息，则返回 true，否则返回 false。</returns>
    public override bool UnPack(IMemoryOwner<byte> memoryOwner, out APackInfo packInfo)
    {
        packInfo = null;
        // 将 IMemoryOwner<byte> 对象的内存资源转换为 Span<byte>
        var memorySpan = memoryOwner.Memory.Span;
        // 如果内存资源中的数据长度小于内部消息头的长度，无法解析
        if (memorySpan.Length < Packet.InnerPacketHeadLength)
        {
            return false;
        }
            
        _messagePacketLength = BitConverter.ToInt32(memorySpan);

        // 检查消息体长度是否超出限制
        if (_messagePacketLength > Packet.PacketBodyMaxLength)
        {
            throw new ScanException($"The received information exceeds the maximum limit = {_messagePacketLength}");
        }
        
        // 如果内存资源中的数据长度小于消息体的长度，无法解析
        if (_messagePacketLength < 0 || memorySpan.Length < _messagePacketLength)
        {
            return false;
        }

        // 创建内部数据包信息实例
        packInfo = InnerPackInfo.Create(memoryOwner);
        packInfo.MessagePacketLength = _messagePacketLength;
        packInfo.ProtocolCode = BitConverter.ToUInt32(memorySpan[Packet.PacketLength..]);
        packInfo.RpcId = BitConverter.ToUInt32(memorySpan[Packet.OuterPacketRpcIdLocation..]);
        packInfo.RouteId = BitConverter.ToInt64(memorySpan[Packet.InnerPacketRouteRouteIdLocation..]);
        return true;
    }

    /// <summary>
    /// 将数据打包成一个内部数据包的内存流。
    /// </summary>
    /// <param name="rpcId">RPC ID。</param>
    /// <param name="routeId">Route ID。</param>
    /// <param name="memoryStream">要打包的数据内存流。</param>
    /// <returns>打包后的内存流。</returns>
    public static MemoryStream Pack(uint rpcId, long routeId, MemoryStream memoryStream)
    {
        // 设置内存流的写入位置，写入 RPC ID 和 Route ID
        memoryStream.Seek(Packet.InnerPacketRpcIdLocation, SeekOrigin.Begin);
        memoryStream.Write(BitConverter.GetBytes(rpcId));
        memoryStream.Write(BitConverter.GetBytes(routeId));
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    /// <summary>
    /// 将数据打包成一个内部数据包的内存流。
    /// </summary>
    /// <param name="rpcId">RPC ID。</param>
    /// <param name="routeId">Route ID。</param>
    /// <param name="message">要打包的数据对象。</param>
    /// <returns>打包后的内存流。</returns>
    public static MemoryStream Pack(uint rpcId, long routeId, object message)
    {
        // 设置默认操作码和消息体长度
        var opCode = Opcode.PingRequest;
        var packetBodyCount = 0;
        // 创建可回收的内存流
        var memoryStream = MemoryStreamHelper.GetRecyclableMemoryStream();
        // 将写入位置设置为消息体的起始位置
        memoryStream.Seek(Packet.InnerPacketHeadLength, SeekOrigin.Begin);

        if (message != null)
        {
            if (message is IBsonMessage)
            {
                try
                {
                    // 使用 MongoHelper 将数据序列化到内存流中
                    MongoHelper.Instance.SerializeTo(message, memoryStream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
            else
            {
                // 使用 ProtoBufHelper 将数据序列化到内存流中
                ProtoBufHelper.ToStream(message, memoryStream);
            }

            // 获取数据对象的操作码并计算消息体的长度
            opCode = MessageDispatcherSystem.Instance.GetOpCode(message.GetType());
            packetBodyCount = (int)(memoryStream.Position - Packet.InnerPacketHeadLength);
            // 如果消息是对象池的需要执行Dispose
            if (message is IPoolMessage iPoolMessage)
            {
                iPoolMessage.Dispose();
            }
        }

        // 检查消息体长度是否超出限制
        if (packetBodyCount > Packet.PacketBodyMaxLength)
        {
            throw new Exception($"Message content exceeds {Packet.PacketBodyMaxLength} bytes");
        }

        // 设置内存流的写入位置，写入消息体长度、操作码、RPC ID 和 Route ID
        memoryStream.Seek(0, SeekOrigin.Begin);
        memoryStream.Write(BitConverter.GetBytes(packetBodyCount));
        memoryStream.Write(BitConverter.GetBytes(opCode));
        memoryStream.Write(BitConverter.GetBytes(rpcId));
        memoryStream.Write(BitConverter.GetBytes(routeId));
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    /// <summary>
    /// 释放资源并进行清理操作。
    /// </summary>
    public override void Dispose()
    {
        _messagePacketLength = 0;
        base.Dispose();
    }
}
#endif