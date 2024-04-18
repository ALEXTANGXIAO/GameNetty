using ProtoBuf;
using Unity.Mathematics;
using System.Collections.Generic;
using GameServer;
#pragma warning disable CS8618

namespace GameServer
{	
	/// <summary>
	///  Gate跟Map服务器进行通讯、注册Address协议。
	/// </summary>
	[ProtoContract]
	public partial class I_G2M_LoginAddressRequest : AProto, IRouteRequest
	{
		[ProtoIgnore]
		public I_M2G_LoginAddressResponse ResponseType { get; set; }
		public uint OpCode() { return InnerOpcode.I_G2M_LoginAddressRequest; }
		public long RouteTypeOpCode() { return CoreRouteType.Route; }
		///<summary>
		/// 用来关联Address的Id,一般是账号Id或UnitId这些不会变动的。
		///</summary>
		[ProtoMember(1)]
		public long AddressableId { get; set; }
	}
	[ProtoContract]
	public partial class I_M2G_LoginAddressResponse : AProto, IRouteResponse
	{
		public uint OpCode() { return InnerOpcode.I_M2G_LoginAddressResponse; }
		[ProtoMember(91, IsRequired = true)]
		public uint ErrorCode { get; set; }
		[ProtoMember(1)]
		public long AddressableId { get; set; }
	}
}
