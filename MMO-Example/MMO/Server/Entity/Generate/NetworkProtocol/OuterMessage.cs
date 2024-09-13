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
	public partial class RealmInfo : AMessage, IProto
	{
		public static RealmInfo Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RealmInfo>();
		}
		public override void Dispose()
		{
			RegionId = default;
			ZoneId = default;
			ZoneName = default;
			RealmAdress = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RealmInfo>(this);
#endif
		}
		[ProtoMember(1)]
		public int RegionId { get; set; }
		[ProtoMember(2)]
		public int ZoneId { get; set; }
		[ProtoMember(3)]
		public string ZoneName { get; set; }
		[ProtoMember(4)]
		public string RealmAdress { get; set; }
	}
	[ProtoContract]
	public partial class C2R_GetZoneList : AMessage, IRequest, IProto
	{
		public static C2R_GetZoneList Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2R_GetZoneList>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2R_GetZoneList>(this);
#endif
		}
		[ProtoIgnore]
		public R2C_GetZoneList ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2R_GetZoneList; }
	}
	[ProtoContract]
	public partial class R2C_GetZoneList : AMessage, IResponse, IProto
	{
		public static R2C_GetZoneList Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<R2C_GetZoneList>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			ZoneId.Clear();
			ZoneName.Clear();
			ZoneState.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<R2C_GetZoneList>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.R2C_GetZoneList; }
		[ProtoMember(1)]
		public List<uint> ZoneId = new List<uint>();
		[ProtoMember(2)]
		public List<string> ZoneName = new List<string>();
		[ProtoMember(3)]
		public List<int> ZoneState = new List<int>();
		[ProtoMember(4)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  注册账号
	/// </summary>
	[ProtoContract]
	public partial class C2R_RegisterRequest : AMessage, IRequest, IProto
	{
		public static C2R_RegisterRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2R_RegisterRequest>();
		}
		public override void Dispose()
		{
			AuthName = default;
			Pw = default;
			Pw2 = default;
			ZoneId = default;
			Version = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2R_RegisterRequest>(this);
#endif
		}
		[ProtoIgnore]
		public R2C_RegisterResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2R_RegisterRequest; }
		[ProtoMember(1)]
		public string AuthName { get; set; }
		[ProtoMember(2)]
		public string Pw { get; set; }
		[ProtoMember(3)]
		public string Pw2 { get; set; }
		[ProtoMember(4)]
		public uint ZoneId { get; set; }
		[ProtoMember(5)]
		public string Version { get; set; }
	}
	[ProtoContract]
	public partial class R2C_RegisterResponse : AMessage, IResponse, IProto
	{
		public static R2C_RegisterResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<R2C_RegisterResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<R2C_RegisterResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.R2C_RegisterResponse; }
		[ProtoMember(1)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  登录账号
	/// </summary>
	[ProtoContract]
	public partial class C2R_LoginRequest : AMessage, IRequest, IProto
	{
		public static C2R_LoginRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2R_LoginRequest>();
		}
		public override void Dispose()
		{
			AuthName = default;
			Pw = default;
			Version = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2R_LoginRequest>(this);
#endif
		}
		[ProtoIgnore]
		public R2C_LoginResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2R_LoginRequest; }
		[ProtoMember(1)]
		public string AuthName { get; set; }
		[ProtoMember(2)]
		public string Pw { get; set; }
		[ProtoMember(3)]
		public string Version { get; set; }
	}
	[ProtoContract]
	public partial class R2C_LoginResponse : AMessage, IResponse, IProto
	{
		public static R2C_LoginResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<R2C_LoginResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			Message = default;
			GateAddress = default;
			GatePort = default;
			Key = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<R2C_LoginResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.R2C_LoginResponse; }
		[ProtoMember(1)]
		public string Message { get; set; }
		[ProtoMember(2)]
		public string GateAddress { get; set; }
		[ProtoMember(3)]
		public int GatePort { get; set; }
		[ProtoMember(4)]
		public long Key { get; set; }
		[ProtoMember(5)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  登录网关
	/// </summary>
	[ProtoContract]
	public partial class C2G_LoginGateRequest : AMessage, IRequest, IProto
	{
		public static C2G_LoginGateRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2G_LoginGateRequest>();
		}
		public override void Dispose()
		{
			AuthName = default;
			Key = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2G_LoginGateRequest>(this);
#endif
		}
		[ProtoIgnore]
		public G2C_LoginGateResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2G_LoginGateRequest; }
		[ProtoMember(1)]
		public string AuthName { get; set; }
		[ProtoMember(2)]
		public long Key { get; set; }
	}
	[ProtoContract]
	public partial class G2C_LoginGateResponse : AMessage, IResponse, IProto
	{
		public static G2C_LoginGateResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2C_LoginGateResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2C_LoginGateResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.G2C_LoginGateResponse; }
		[ProtoMember(1)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  创建角色 消息请求
	/// </summary>
	[ProtoContract]
	public partial class C2G_CreateRoleRequest : AMessage, IRequest, IProto
	{
		public static C2G_CreateRoleRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2G_CreateRoleRequest>();
		}
		public override void Dispose()
		{
			UnitConfigId = default;
			Sex = default;
			NickName = default;
			Class = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2G_CreateRoleRequest>(this);
#endif
		}
		[ProtoIgnore]
		public G2C_CreateRoleResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2G_CreateRoleRequest; }
		[ProtoMember(1)]
		public int UnitConfigId { get; set; }
		[ProtoMember(2)]
		public int Sex { get; set; }
		[ProtoMember(3)]
		public string NickName { get; set; }
		[ProtoMember(4)]
		public string Class { get; set; }
	}
	/// <summary>
	///  创建角色 消息响应
	/// </summary>
	[ProtoContract]
	public partial class G2C_CreateRoleResponse : AMessage, IResponse, IProto
	{
		public static G2C_CreateRoleResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2C_CreateRoleResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			RoleInfo = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2C_CreateRoleResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.G2C_CreateRoleResponse; }
		[ProtoMember(1)]
		public RoleInfo RoleInfo { get; set; }
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  删除角色 消息请求
	/// </summary>
	[ProtoContract]
	public partial class C2G_RoleDeleteRequest : AMessage, IRequest, IProto
	{
		public static C2G_RoleDeleteRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2G_RoleDeleteRequest>();
		}
		public override void Dispose()
		{
			RoleId = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2G_RoleDeleteRequest>(this);
#endif
		}
		[ProtoIgnore]
		public G2C_RoleDeleteResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2G_RoleDeleteRequest; }
		[ProtoMember(1)]
		public long RoleId { get; set; }
	}
	/// <summary>
	///  删除角色 消息响应
	/// </summary>
	[ProtoContract]
	public partial class G2C_RoleDeleteResponse : AMessage, IResponse, IProto
	{
		public static G2C_RoleDeleteResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2C_RoleDeleteResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2C_RoleDeleteResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.G2C_RoleDeleteResponse; }
		[ProtoMember(1)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  获取角色列列表 消息请求
	/// </summary>
	[ProtoContract]
	public partial class C2G_RoleListRequest : AMessage, IRequest, IProto
	{
		public static C2G_RoleListRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2G_RoleListRequest>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2G_RoleListRequest>(this);
#endif
		}
		[ProtoIgnore]
		public G2C_RoleListResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2G_RoleListRequest; }
	}
	/// <summary>
	///  角色列表信息 消息响应
	/// </summary>
	[ProtoContract]
	public partial class G2C_RoleListResponse : AMessage, IResponse, IProto
	{
		public static G2C_RoleListResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2C_RoleListResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			RoleInfos.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2C_RoleListResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.G2C_RoleListResponse; }
		[ProtoMember(1)]
		public List<RoleInfo> RoleInfos = new List<RoleInfo>();
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  角色列表子项信息
	/// </summary>
	[ProtoContract]
	public partial class RoleInfo : AMessage, IProto
	{
		public static RoleInfo Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RoleInfo>();
		}
		public override void Dispose()
		{
			UnitConfigId = default;
			AccountId = default;
			UnitType = default;
			RoleId = default;
			Sex = default;
			NickName = default;
			Level = default;
			CreatedTime = default;
			Experience = default;
			ClassName = default;
			LastMap = default;
			LastMoveInfo = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RoleInfo>(this);
#endif
		}
		[ProtoMember(1)]
		public int UnitConfigId { get; set; }
		[ProtoMember(2)]
		public long AccountId { get; set; }
		[ProtoMember(3)]
		public int UnitType { get; set; }
		///<summary>
		/// 角色ID
		///</summary>
		[ProtoMember(4)]
		public long RoleId { get; set; }
		///<summary>
		/// 性别
		///</summary>
		[ProtoMember(5)]
		public int Sex { get; set; }
		///<summary>
		/// 昵称
		///</summary>
		[ProtoMember(6)]
		public string NickName { get; set; }
		///<summary>
		/// 年龄
		///</summary>
		[ProtoMember(7)]
		public int Level { get; set; }
		///<summary>
		/// 创建时间
		///</summary>
		[ProtoMember(8)]
		public long CreatedTime { get; set; }
		///<summary>
		/// 经验
		///</summary>
		[ProtoMember(9)]
		public long Experience { get; set; }
		///<summary>
		/// 职业
		///</summary>
		[ProtoMember(10)]
		public string ClassName { get; set; }
		[ProtoMember(11)]
		public int LastMap { get; set; }
		[ProtoMember(12)]
		public MoveInfo LastMoveInfo { get; set; }
		///<summary>
		/// repeated ItemInfo Equipments = 11;
		///</summary>
	}
	/// <summary>
	///  进入地图
	/// </summary>
	[ProtoContract]
	public partial class C2G_EnterMapRequest : AMessage, IRequest, IProto
	{
		public static C2G_EnterMapRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2G_EnterMapRequest>();
		}
		public override void Dispose()
		{
			RoleId = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2G_EnterMapRequest>(this);
#endif
		}
		[ProtoIgnore]
		public G2C_EnterMapResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2G_EnterMapRequest; }
		[ProtoMember(1)]
		public long RoleId { get; set; }
	}
	[ProtoContract]
	public partial class G2C_EnterMapResponse : AMessage, IResponse, IProto
	{
		public static G2C_EnterMapResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2C_EnterMapResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			MapNum = default;
			LastMoveInfo = default;
			RoleInfo = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2C_EnterMapResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.G2C_EnterMapResponse; }
		[ProtoMember(1)]
		public int MapNum { get; set; }
		[ProtoMember(2)]
		public MoveInfo LastMoveInfo { get; set; }
		[ProtoMember(3)]
		public RoleInfo RoleInfo { get; set; }
		[ProtoMember(4)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  强制断开连接通知
	/// </summary>
	[ProtoContract]
	public partial class G2C_ForceDisconnected : AMessage, IMessage, IProto
	{
		public static G2C_ForceDisconnected Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<G2C_ForceDisconnected>();
		}
		public override void Dispose()
		{
			Message = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<G2C_ForceDisconnected>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.G2C_ForceDisconnected; }
		[ProtoMember(1)]
		public string Message { get; set; }
	}
	/// <summary>
	///  退出地图
	/// </summary>
	[ProtoContract]
	public partial class C2M_ExitRequest : AMessage, IAddressableRouteRequest, IProto
	{
		public static C2M_ExitRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2M_ExitRequest>();
		}
		public override void Dispose()
		{
			Message = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2M_ExitRequest>(this);
#endif
		}
		[ProtoIgnore]
		public M2C_ExitResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.C2M_ExitRequest; }
		[ProtoMember(1)]
		public string Message { get; set; }
	}
	[ProtoContract]
	public partial class M2C_ExitResponse : AMessage, IAddressableRouteResponse, IProto
	{
		public static M2C_ExitResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<M2C_ExitResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			Message = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<M2C_ExitResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.M2C_ExitResponse; }
		[ProtoMember(1)]
		public string Message { get; set; }
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	/// <summary>
	///  位置对象
	/// </summary>
	[ProtoContract]
	public partial class MoveInfo : AMessage, IProto
	{
		public static MoveInfo Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<MoveInfo>();
		}
		public override void Dispose()
		{
			Position = default;
			Rotation = default;
			RoleId = default;
			MoveState = default;
			MoveStartTime = default;
			MoveEndTime = default;
			Flag = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<MoveInfo>(this);
#endif
		}
		[ProtoMember(1)]
		public Position Position { get; set; }
		[ProtoMember(2)]
		public Rotation Rotation { get; set; }
		[ProtoMember(3)]
		public long RoleId { get; set; }
		[ProtoMember(4)]
		public int MoveState { get; set; }
		[ProtoMember(5)]
		public long MoveStartTime { get; set; }
		[ProtoMember(6)]
		public long MoveEndTime { get; set; }
		[ProtoMember(7)]
		public long Flag { get; set; }
	}
	[ProtoContract]
	public partial class Position : AMessage, IProto
	{
		public static Position Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<Position>();
		}
		public override void Dispose()
		{
			X = default;
			Y = default;
			Z = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<Position>(this);
#endif
		}
		[ProtoMember(1)]
		public float X { get; set; }
		[ProtoMember(2)]
		public float Y { get; set; }
		[ProtoMember(3)]
		public float Z { get; set; }
	}
	[ProtoContract]
	public partial class Rotation : AMessage, IProto
	{
		public static Rotation Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<Rotation>();
		}
		public override void Dispose()
		{
			RotA = default;
			RotB = default;
			RotC = default;
			RotW = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<Rotation>(this);
#endif
		}
		[ProtoMember(1)]
		public float RotA { get; set; }
		[ProtoMember(2)]
		public float RotB { get; set; }
		[ProtoMember(3)]
		public float RotC { get; set; }
		[ProtoMember(4)]
		public float RotW { get; set; }
	}
	/// <summary>
	///  移动操作
	/// </summary>
	[ProtoContract]
	public partial class C2M_StartMoveMessage : AMessage, IAddressableRouteMessage, IProto
	{
		public static C2M_StartMoveMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2M_StartMoveMessage>();
		}
		public override void Dispose()
		{
			MoveInfo = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2M_StartMoveMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.C2M_StartMoveMessage; }
		[ProtoMember(1)]
		public MoveInfo MoveInfo { get; set; }
	}
	[ProtoContract]
	public partial class C2M_StopMoveMessage : AMessage, IAddressableRouteMessage, IProto
	{
		public static C2M_StopMoveMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<C2M_StopMoveMessage>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<C2M_StopMoveMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.C2M_StopMoveMessage; }
	}
	/// <summary>
	///  核心状态同步
	/// </summary>
	[ProtoContract]
	public partial class M2C_MoveBroadcast : AMessage, IAddressableRouteMessage, IProto
	{
		public static M2C_MoveBroadcast Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<M2C_MoveBroadcast>();
		}
		public override void Dispose()
		{
			Moves.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<M2C_MoveBroadcast>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.M2C_MoveBroadcast; }
		[ProtoMember(1)]
		public List<MoveInfo> Moves = new List<MoveInfo>();
	}
	[ProtoContract]
	public partial class M2C_UnitCreate : AMessage, IAddressableRouteMessage, IProto
	{
		public static M2C_UnitCreate Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<M2C_UnitCreate>();
		}
		public override void Dispose()
		{
			UnitInfos.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<M2C_UnitCreate>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.M2C_UnitCreate; }
		[ProtoMember(1)]
		public List<RoleInfo> UnitInfos = new List<RoleInfo>();
	}
	[ProtoContract]
	public partial class M2C_UnitRemove : AMessage, IAddressableRouteMessage, IProto
	{
		public static M2C_UnitRemove Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<M2C_UnitRemove>();
		}
		public override void Dispose()
		{
			UnitIds.Clear();
			RoleIds.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<M2C_UnitRemove>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.M2C_UnitRemove; }
		[ProtoMember(1)]
		public List<long> UnitIds = new List<long>();
		[ProtoMember(2)]
		public List<long> RoleIds = new List<long>();
	}
}
