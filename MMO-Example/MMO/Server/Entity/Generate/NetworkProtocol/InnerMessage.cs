using ProtoBuf;

using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Fantasy;
using Fantasy.Network.Interface;
using Fantasy.Serialize;
// ReSharper disable InconsistentNaming
// ReSharper disable RedundantUsingDirective
// ReSharper disable RedundantOverriddenMember
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8618

namespace Fantasy
{	
	[ProtoContract]
	public partial class S2S_ConnectRequest : AMessage, IRouteRequest, IProto
	{
		public static S2S_ConnectRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<S2S_ConnectRequest>();
		}
		public override void Dispose()
		{
			Key = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<S2S_ConnectRequest>(this);
#endif
		}
		[ProtoIgnore]
		public S2S_ConnectResponse ResponseType { get; set; }
		public uint OpCode() { return InnerOpcode.S2S_ConnectRequest; }
		[ProtoMember(1)]
		public int Key { get; set; }
	}
	[ProtoContract]
	public partial class S2S_ConnectResponse : AMessage, IRouteResponse, IProto
	{
		public static S2S_ConnectResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<S2S_ConnectResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			Key = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<S2S_ConnectResponse>(this);
#endif
		}
		public uint OpCode() { return InnerOpcode.S2S_ConnectResponse; }
		[ProtoMember(1)]
		public int Key { get; set; }
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class R2G_GetLoginKeyRequest : AMessage, IRouteRequest, IProto
	{
		public static R2G_GetLoginKeyRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<R2G_GetLoginKeyRequest>();
		}
		public override void Dispose()
		{
			AuthName = default;
			AccountId = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<R2G_GetLoginKeyRequest>(this);
#endif
		}
		[ProtoIgnore]
		public G2R_GetLoginKeyResponse ResponseType { get; set; }
		public uint OpCode() { return InnerOpcode.R2G_GetLoginKeyRequest; }
		[ProtoMember(1)]
		public string AuthName { get; set; }
		[ProtoMember(2)]
		public long AccountId { get; set; }
	}
	[ProtoContract]
	public partial class G2R_GetLoginKeyResponse : AMessage, IRouteResponse, IProto
	{
		public static G2R_GetLoginKeyResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2R_GetLoginKeyResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			Key = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2R_GetLoginKeyResponse>(this);
#endif
		}
		public uint OpCode() { return InnerOpcode.G2R_GetLoginKeyResponse; }
		[ProtoMember(1)]
		public long Key { get; set; }
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  进入地图
	/// </summary>
	[ProtoContract]
	public partial class G2M_CreateUnitRequest : AMessage, IRouteRequest, IProto
	{
		public static G2M_CreateUnitRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2M_CreateUnitRequest>();
		}
		public override void Dispose()
		{
			Message = default;
			PlayerId = default;
			SessionRuntimeId = default;
			GateRouteId = default;
			RoleInfo = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2M_CreateUnitRequest>(this);
#endif
		}
		[ProtoIgnore]
		public M2G_CreateUnitResponse ResponseType { get; set; }
		public uint OpCode() { return InnerOpcode.G2M_CreateUnitRequest; }
		[ProtoMember(1)]
		public string Message { get; set; }
		[ProtoMember(2)]
		public long PlayerId { get; set; }
		[ProtoMember(3)]
		public long SessionRuntimeId { get; set; }
		[ProtoMember(4)]
		public long GateRouteId { get; set; }
		[ProtoMember(5)]
		public RoleInfo RoleInfo { get; set; }
	}
	[ProtoContract]
	public partial class M2G_CreateUnitResponse : AMessage, IRouteResponse, IProto
	{
		public static M2G_CreateUnitResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<M2G_CreateUnitResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			Message = default;
			AddressableId = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<M2G_CreateUnitResponse>(this);
#endif
		}
		public uint OpCode() { return InnerOpcode.M2G_CreateUnitResponse; }
		[ProtoMember(1)]
		public string Message { get; set; }
		[ProtoMember(2)]
		public long AddressableId { get; set; }
		///<summary>
		/// Unit Unit = 3;
		///</summary>
		[ProtoMember(3)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class G2M_Return2MapMsg : AMessage, IAddressableRouteMessage, IProto
	{
		public static G2M_Return2MapMsg Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2M_Return2MapMsg>();
		}
		public override void Dispose()
		{
			MapNum = default;
			RoleInfo = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2M_Return2MapMsg>(this);
#endif
		}
		public uint OpCode() { return InnerOpcode.G2M_Return2MapMsg; }
		[ProtoMember(1)]
		public int MapNum { get; set; }
		[ProtoMember(2)]
		public RoleInfo RoleInfo { get; set; }
	}
	[ProtoContract]
	public partial class G2M_SessionDisconnectMsg : AMessage, IAddressableRouteMessage, IProto
	{
		public static G2M_SessionDisconnectMsg Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2M_SessionDisconnectMsg>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2M_SessionDisconnectMsg>(this);
#endif
		}
		public uint OpCode() { return InnerOpcode.G2M_SessionDisconnectMsg; }
	}
	[ProtoContract]
	public partial class M2G_QuitMapMsg : AMessage, IRouteMessage, IProto
	{
		public static M2G_QuitMapMsg Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<M2G_QuitMapMsg>();
		}
		public override void Dispose()
		{
			AccountId = default;
			MapNum = default;
			QuitGame = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<M2G_QuitMapMsg>(this);
#endif
		}
		public uint OpCode() { return InnerOpcode.M2G_QuitMapMsg; }
		[ProtoMember(1)]
		public long AccountId { get; set; }
		[ProtoMember(2)]
		public int MapNum { get; set; }
		[ProtoMember(3)]
		public bool QuitGame { get; set; }
	}
	[ProtoContract]
	public partial class S2Mgr_ServerStartComplete : AMessage, IRouteMessage, IProto
	{
		public static S2Mgr_ServerStartComplete Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<S2Mgr_ServerStartComplete>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<S2Mgr_ServerStartComplete>(this);
#endif
		}
		public uint OpCode() { return InnerOpcode.S2Mgr_ServerStartComplete; }
	}
	[ProtoContract]
	public partial class Mgr2R_MachineStartFinished : AMessage, IRouteMessage, IProto
	{
		public static Mgr2R_MachineStartFinished Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<Mgr2R_MachineStartFinished>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<Mgr2R_MachineStartFinished>(this);
#endif
		}
		public uint OpCode() { return InnerOpcode.Mgr2R_MachineStartFinished; }
	}
	[ProtoContract]
	public partial class AccountMsg : AMessage, IProto
	{
		public static AccountMsg Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<AccountMsg>();
		}
		public override void Dispose()
		{
			AuthName = default;
			AccountId = default;
			Pw = default;
			LastLoginTime = default;
			LastLoginIp = default;
			RegisterTime = default;
			RegisterIp = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<AccountMsg>(this);
#endif
		}
		[ProtoMember(1)]
		public string AuthName { get; set; }
		[ProtoMember(2)]
		public long AccountId { get; set; }
		[ProtoMember(3)]
		public string Pw { get; set; }
		[ProtoMember(4)]
		public long LastLoginTime { get; set; }
		[ProtoMember(5)]
		public string LastLoginIp { get; set; }
		[ProtoMember(6)]
		public long RegisterTime { get; set; }
		[ProtoMember(7)]
		public string RegisterIp { get; set; }
	}
}
