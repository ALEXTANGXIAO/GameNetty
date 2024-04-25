using System;
using System.IO;
using System.Net.Sockets;

// ReSharper disable InconsistentNaming
#pragma warning disable CS8622
#pragma warning disable CS8601
#pragma warning disable CS8618

namespace GameNetty
{
    /// <summary>
    /// TCP 服务器网络通道，用于处理服务器与客户端之间的数据通信。
    /// </summary>
    public sealed class TCPServerNetworkChannel : ANetworkChannel
    {
        #region 网络主线程

        private bool _isSending;
        private readonly Socket _socket;
        private readonly CircularBuffer _sendBuffer = new CircularBuffer();
        private readonly CircularBuffer _receiveBuffer = new CircularBuffer();
        private readonly SocketAsyncEventArgs _outArgs = new SocketAsyncEventArgs();
        private readonly SocketAsyncEventArgs _innArgs = new SocketAsyncEventArgs();

        /// <summary>
        /// 当通道被释放时触发的事件。
        /// </summary>
        public override event Action OnDispose;
        /// <summary>
        /// 当接收到内存流数据时触发的事件。
        /// </summary>
        public override event Action<APackInfo> OnReceiveMemoryStream;

        /// <summary>
        /// 初始化 TCPServerNetworkChannel 实例。
        /// </summary>
        /// <param name="id">通道 ID。</param>
        /// <param name="socket">与客户端连接的 Socket。</param>
        /// <param name="network">所属的网络实例。</param>
        public TCPServerNetworkChannel(uint id, Socket socket, ANetwork network) : base(network.Scene, id, network.Id)
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            _socket = socket;
            _socket.NoDelay = true;
            RemoteEndPoint = _socket.RemoteEndPoint;

            _innArgs.Completed += OnComplete;
            _outArgs.Completed += OnComplete;

            PacketParser = APacketParser.CreatePacketParser(network.NetworkTarget);
        }

        /// <summary>
        /// 释放资源并关闭通道。
        /// </summary>
        public override void Dispose()
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            if (IsDisposed)
            {
                return;
            }

            _isSending = false;

            if (_socket.Connected)
            {
                _socket.Disconnect(true);
                _socket.Close();
            }
            
            _outArgs.Dispose();
            _innArgs.Dispose();
            _sendBuffer.Dispose();
            _receiveBuffer.Dispose();
            ThreadSynchronizationContext.Main.Post(OnDispose);
            base.Dispose();
        }

        /// <summary>
        /// 向通道发送内存流数据。
        /// </summary>
        /// <param name="memoryStream">待发送的内存流。</param>
        public void Send(MemoryStream memoryStream)
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            _sendBuffer.Write(memoryStream);
            
            // 因为memoryStream对象池出来的、所以需要手动回收下
            
            memoryStream.Dispose();
            
            if (_isSending)
            {
                return;
            }
            
            Send();
        }

        private void Send()
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            if (_isSending || IsDisposed)
            {
                return;
            }

            for (;;)
            {
                try
                {
                    if (IsDisposed)
                    {
                        return;
                    }
                    
                    if (_sendBuffer.Length == 0)
                    {
                        _isSending = false;
                        return;
                    }
                    
                    _isSending = true;
                    
                    var sendSize = CircularBuffer.ChunkSize - _sendBuffer.FirstIndex;
                    
                    if (sendSize > _sendBuffer.Length)
                    {
                        sendSize = (int) _sendBuffer.Length;
                    }

                    _outArgs.SetBuffer(_sendBuffer.First, _sendBuffer.FirstIndex, sendSize);
                    
                    if (_socket.SendAsync(_outArgs))
                    {
                        return;
                    }
                    
                    SendCompletedHandler(_outArgs);
                }
                catch (Exception e)
                {
                    NettyLog.Error(e);
                }
            }
        }

        private void SendCompletedHandler(SocketAsyncEventArgs asyncEventArgs)
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            if (asyncEventArgs.SocketError != SocketError.Success || asyncEventArgs.BytesTransferred == 0)
            {
                return;
            }
            
            _sendBuffer.FirstIndex += asyncEventArgs.BytesTransferred;
        
            if (_sendBuffer.FirstIndex == CircularBuffer.ChunkSize)
            {
                _sendBuffer.FirstIndex = 0;
                _sendBuffer.RemoveFirst();
            }
        }

        private void OnSendComplete(SocketAsyncEventArgs asyncEventArgs)
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            if (IsDisposed)
            {
                return;
            }
            
            _isSending = false;
            SendCompletedHandler(asyncEventArgs);

            if (_sendBuffer.Length > 0)
            {
                Send();
            }
        }

        /// <summary>
        /// 开始接收数据。
        /// </summary>
        public void Receive()
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            for (;;)
            {
                try
                {
                    if (IsDisposed)
                    {
                        return;
                    }
                    
                    var size = CircularBuffer.ChunkSize - _receiveBuffer.LastIndex;
                    _innArgs.SetBuffer(_receiveBuffer.Last, _receiveBuffer.LastIndex, size);
                
                    if (_socket.ReceiveAsync(_innArgs))
                    {
                        return;
                    }

                    ReceiveCompletedHandler(_innArgs);
                }
                catch (Exception e)
                {
                    NettyLog.Error(e);
                }
            }
        }

        private void ReceiveCompletedHandler(SocketAsyncEventArgs asyncEventArgs)
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            if (asyncEventArgs.SocketError != SocketError.Success)
            {
                return;
            }
            
            if (asyncEventArgs.BytesTransferred == 0)
            {
                Dispose();
                return;
            }
           
            _receiveBuffer.LastIndex += asyncEventArgs.BytesTransferred;
            
            if (_receiveBuffer.LastIndex >= CircularBuffer.ChunkSize)
            {
                _receiveBuffer.AddLast();
                _receiveBuffer.LastIndex = 0;
            }

            for (;;)
            {
                try
                {
                    if (IsDisposed)
                    {
                        return;
                    }
                    
                    if (!PacketParser.UnPack(_receiveBuffer, out var packInfo))
                    {
                        break;
                    }
                    
                    ThreadSynchronizationContext.Main.Post(() =>
                    {
                        if (IsDisposed)
                        {
                            return;
                        }
                        
                        // ReSharper disable once PossibleNullReferenceException
                        OnReceiveMemoryStream(packInfo);
                    });
                }
                catch (ScanException e)
                {
                    NettyLog.Debug($"RemoteAddress:{RemoteEndPoint} \n{e}");
                    Dispose();
                }
                catch (Exception e)
                {
                    NettyLog.Error($"RemoteAddress:{RemoteEndPoint} \n{e}");
                    Dispose();
                }
            }
        }

        private void OnReceiveComplete(SocketAsyncEventArgs asyncEventArgs)
        {
#if GAME_DEVELOP
            if (NetworkThread.Instance.ManagedThreadId != Thread.CurrentThread.ManagedThreadId)
            {
                Log.Error("not in NetworkThread!");
                return;
            }
#endif
            ReceiveCompletedHandler(asyncEventArgs);
            Receive();
        }

        #endregion

        #region 网络线程（由Socket底层产生的线程）

        private void OnComplete(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            if (IsDisposed)
            {
                return;
            }
 
            switch (asyncEventArgs.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                {
                    NetworkThread.Instance.SynchronizationContext.Post(() => OnReceiveComplete(asyncEventArgs));
                    break;
                }
                case SocketAsyncOperation.Send:
                {
                    NetworkThread.Instance.SynchronizationContext.Post(() => OnSendComplete(asyncEventArgs));
                    break;
                }
                case SocketAsyncOperation.Disconnect:
                {
                    NetworkThread.Instance.SynchronizationContext.Post(Dispose);
                    break;
                }
                default:
                {
                    throw new Exception($"Socket Error: {asyncEventArgs.LastOperation}");
                }
            }
        }

        #endregion
    }
}