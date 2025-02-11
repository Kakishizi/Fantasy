using System;
using System.Collections.Generic;
using Fantasy.Async;
using Fantasy.Entitas;
using Fantasy.Event;
using Fantasy.IdFactory;
using Fantasy.Network;
using Fantasy.Network.Interface;
using Fantasy.Scheduler;
using Fantasy.Timer;
#if FANTASY_NET
using Fantasy.DataBase;
using Fantasy.Platform.Net;
using Fantasy.SingleCollection;
using System.Runtime.CompilerServices;
using Fantasy.Network.Route;
#endif
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
namespace Fantasy
{
    internal class SceneRuntimeType
    {
        /// <summary>
        /// Scene在主线程中运行.
        /// </summary>
        public const string MainThread = "MainThread";
        /// <summary>
        /// Scene在一个独立的线程中运行.
        /// </summary>
        public const string MultiThread = "MultiThread";
        /// <summary>
        /// Scene在一个根据当前CPU核心数创建的线程池中运行.
        /// </summary>
        public const string ThreadPool = "ThreadPool";
    }
    /// <summary>
    /// 表示一个场景实体，用于创建与管理特定的游戏场景信息。
    /// </summary>
    public class Scene : Entity
    {
        #region Members
#if FANTASY_NET
        /// <summary>
        /// Scene类型，对应SceneConfig的SceneType
        /// </summary>
        public int SceneType { get; private set; }
        /// <summary>
        /// 所属的世界
        /// </summary>
        public World World { get; private set; }
        /// <summary>
        /// 所在的Process
        /// </summary>
        public Process Process { get; private set; }
        /// <summary>
        /// SceneConfig的Id
        /// </summary>
        public uint SceneConfigId { get; private set; }
        internal ANetwork InnerNetwork { get; private set; }
        internal ANetwork OuterNetwork { get; private set; }
        internal SceneConfig SceneConfig => SceneConfigData.Instance.Get(SceneConfigId);
        private readonly Dictionary<uint, ProcessSessionInfo> _processSessionInfos = new Dictionary<uint, ProcessSessionInfo>();
#endif
        internal ThreadSynchronizationContext ThreadSynchronizationContext { get; private set; }
        private readonly Dictionary<long, Entity> _entities = new Dictionary<long, Entity>();
        #endregion

        #region IdFactory

        /// <summary>
        /// Entity实体Id的生成器
        /// </summary>
        public EntityIdFactory EntityIdFactory { get; private set; }
        /// <summary>
        /// Entity实体RuntimeId的生成器
        /// </summary>
        public RuntimeIdFactory RuntimeIdFactory { get; private set; }

        #endregion
        
        #region Pool

        internal EntityPool EntityPool { get; private set; }
        internal EntityListPool<Entity> EntityListPool { get; private set; }
        internal EntitySortedDictionaryPool<long, Entity> EntitySortedDictionaryPool { get; private set; }

        #endregion
        
        #region Component

        /// <summary>
        /// Scene下的任务调度器系统组件
        /// </summary>
        public TimerComponent TimerComponent { get; private set; }
        /// <summary>
        /// Scene下的事件系统组件
        /// </summary>
        public EventComponent EventComponent { get; private set; }
        /// <summary>
        /// Scene下的ESC系统组件
        /// </summary>
        public EntityComponent EntityComponent { get; private set; }
        /// <summary>
        /// Scene下的网络消息对象池组件
        /// </summary>
        public MessagePoolComponent MessagePoolComponent { get; private set; }
        /// <summary>
        /// Scene下的协程锁组件
        /// </summary>
        public CoroutineLockComponent CoroutineLockComponent { get; private set; }
        internal MessageDispatcherComponent MessageDispatcherComponent { get; private set; }
        /// <summary>
        /// Scene下的内网消息发送组件
        /// </summary>
        public NetworkMessagingComponent NetworkMessagingComponent { get; private set; }
#if FANTASY_NET
        /// <summary>
        /// Scene下的Entity分表组件
        /// </summary>
        public SingleCollectionComponent SingleCollectionComponent { get; private set; }
#endif
        #endregion

        #region Initialize

        private async FTask Initialize()
        {
            EntityPool = new EntityPool();
            EntityListPool = new EntityListPool<Entity>();
            EntitySortedDictionaryPool = new EntitySortedDictionaryPool<long, Entity>();
            SceneUpdate = EntityComponent = await Create<EntityComponent>(this, false, false).Initialize();
            MessagePoolComponent = AddComponent<MessagePoolComponent>(false);
            EventComponent = await AddComponent<EventComponent>(false).Initialize();
            TimerComponent = AddComponent<TimerComponent>(false).Initialize();
            CoroutineLockComponent = AddComponent<CoroutineLockComponent>(false).Initialize();
            MessageDispatcherComponent = await AddComponent<MessageDispatcherComponent>(false).Initialize();
            NetworkMessagingComponent = AddComponent<NetworkMessagingComponent>(false);
#if FANTASY_NET
            SingleCollectionComponent = await AddComponent<SingleCollectionComponent>(false).Initialize();
#endif
        }

        private void Initialize(Scene scene) 
        {
            scene.EntityPool = scene.EntityPool;
            scene.EntityListPool = scene.EntityListPool;
            scene.EntitySortedDictionaryPool = scene.EntitySortedDictionaryPool;
            SceneUpdate = scene.SceneUpdate;
            TimerComponent = scene.TimerComponent;
            EventComponent = scene.EventComponent;
            EntityComponent = scene.EntityComponent;
            MessagePoolComponent = scene.MessagePoolComponent;
            CoroutineLockComponent = scene.CoroutineLockComponent;
            MessageDispatcherComponent = scene.MessageDispatcherComponent;
            NetworkMessagingComponent = scene.NetworkMessagingComponent;
#if FANTASY_NET
            SingleCollectionComponent = scene.SingleCollectionComponent;
#endif
        }
        /// <summary>
        /// Scene销毁方法，执行了该方法会把当前Scene下的所有实体都销毁掉。
        /// </summary>
        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
#if FANTASY_NET
            foreach (var (_, innerSession) in _processSessionInfos)
            {
                innerSession.Dispose();
            }
            _processSessionInfos.Clear();
#endif
#if FANTASY_UNITY
            Session = null;
            _unityWorldId--;
            _unitySceneId--;
            UnityNetwork?.Dispose();
#endif
            EventComponent.Dispose();
            MessagePoolComponent.Dispose();
            EntityPool.Dispose();
            EntityListPool.Dispose();
            EntitySortedDictionaryPool.Dispose();
            base.Dispose();
        }

        #endregion

        private ISceneUpdate SceneUpdate { get; set; }

        internal void Update()
        {
            try
            {
                SceneUpdate.Update();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        #region Create

#if FANTASY_UNITY
        private static uint _unitySceneId = 0;
        private static byte _unityWorldId = 0;
        public Session Session { get; private set; }
        private AClientNetwork UnityNetwork { get; set; }
        /// <summary>
        /// 创建一个Unity的Scene，注意:该方法只能在主线程下使用。
        /// </summary>
        /// <param name="sceneRuntimeType">选择Scene的运行方式</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async FTask<Scene> Create(string sceneRuntimeType = SceneRuntimeType.MainThread)
        {
            var world = ++_unityWorldId;

            if (world > byte.MaxValue - 1)
            {
                throw new Exception($"World ID ({world}) exceeds the maximum allowed value of 255.");
            }

            var sceneId = (uint)(++_unitySceneId + world * 1000);
            
            if (sceneId > 255255)
            {
                throw new Exception($"Scene ID ({sceneId}) exceeds the maximum allowed value of 255255.");
            }

            var scene = new Scene();
            scene.Scene = scene;
            scene.Parent = scene;
            scene.EntityIdFactory = new EntityIdFactory(sceneId, world);
            scene.RuntimeIdFactory = new RuntimeIdFactory(sceneId, world);
            scene.Id = new EntityIdStruct(0, sceneId, world, 0);
            scene.RunTimeId = new RuntimeIdStruct(0, sceneId, world, 0);
            scene.AddEntity(scene);
            await SetScheduler(scene, null, sceneRuntimeType);
            return scene;
        }
        public Session Connect(string remoteAddress, NetworkProtocolType networkProtocolType, Action onConnectComplete, Action onConnectFail, Action onConnectDisconnect, bool isHttps, int connectTimeout = 5000)
        {
            UnityNetwork?.Dispose();
            UnityNetwork = NetworkProtocolFactory.CreateClient(this, networkProtocolType, NetworkTarget.Outer);
            Session = UnityNetwork.Connect(remoteAddress, onConnectComplete, onConnectFail, onConnectDisconnect, isHttps, connectTimeout);
            return Session;
        }
#endif
#if FANTASY_NET
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Scene Create(Process process, byte worldId, uint sceneConfigId)
        {
            var scene = new Scene();
            scene.Scene = scene;
            scene.Parent = scene;
            scene.Process = process;
            scene.EntityIdFactory = new EntityIdFactory(sceneConfigId, worldId);
            scene.RuntimeIdFactory = new RuntimeIdFactory(sceneConfigId, worldId);
            scene.Id = new EntityIdStruct(0, sceneConfigId, worldId, 0);
            scene.RunTimeId = new RuntimeIdStruct(0, sceneConfigId, worldId, 0);
            scene.AddEntity(scene);
            return scene;
        }
        /// <summary>
        /// 创建一个新的Scene
        /// </summary>
        /// <param name="process">所属的Process</param>
        /// <param name="machineConfig">对应的MachineConfig配置文件</param>
        /// <param name="sceneConfig">对应的SceneConfig配置文件</param>
        /// <returns>创建成功后会返回创建的Scene的实例</returns>
        public static async FTask<Scene> Create(Process process, MachineConfig machineConfig, SceneConfig sceneConfig)
        {
            var scene = Create(process, (byte)sceneConfig.WorldConfigId, sceneConfig.Id);
            scene.SceneType = sceneConfig.SceneType;
            scene.SceneConfigId = sceneConfig.Id;
            await SetScheduler(scene, null, sceneConfig.SceneRuntimeType);
            
            if (sceneConfig.WorldConfigId != 0)
            {
                scene.World = World.Create(scene, (byte)sceneConfig.WorldConfigId);
            }

            if (sceneConfig.InnerPort != 0)
            {
                // 创建内网网络服务器
                scene.InnerNetwork = NetworkProtocolFactory.CreateServer(scene, ProcessDefine.InnerNetwork, NetworkTarget.Inner, machineConfig.InnerBindIP, sceneConfig.InnerPort);
            }

            if (sceneConfig.OuterPort != 0)
            {
                // 创建外网网络服务
                var networkProtocolType = Enum.Parse<NetworkProtocolType>(sceneConfig.NetworkProtocol);
                scene.OuterNetwork = NetworkProtocolFactory.CreateServer(scene, networkProtocolType, NetworkTarget.Outer, machineConfig.OuterBindIP, sceneConfig.OuterPort);
            }
            Process.AddScene(scene);
            process.AddSceneToProcess(scene);
            scene.ThreadSynchronizationContext.Post(() =>
            {
                if (sceneConfig.SceneTypeString == "Addressable")
                {
                    // 如果是AddressableScene,自动添加上AddressableManageComponent。
                    scene.AddComponent<AddressableManageComponent>(); 
                }
                
                scene.EventComponent.PublishAsync(new OnCreateScene(scene)).Coroutine();
            });
            return scene;
        }
        /// <summary>
        /// 在Scene下面创建一个子Scene，一般用于副本，或者一些特殊的场景。
        /// </summary>
        /// <param name="parentScene">主Scene的实例</param>
        /// <param name="sceneType">SceneType，可以在SceneType里找到，例如:SceneType.Addressable</param>
        /// <param name="onSubSceneComplete">子Scene创建成功后执行的委托，可以传递null</param>
        /// <returns></returns>
        public static async FTask<Scene> CreateSubScene(Scene parentScene, int sceneType, Action<Scene, Scene> onSubSceneComplete)
        {
            var scene = new Scene();
            scene.Scene = scene;
            scene.Parent = scene;
            scene.SceneType = sceneType;
            scene.World = parentScene.World;
            scene.Process = parentScene.Process;
            scene.EntityIdFactory = parentScene.EntityIdFactory;
            scene.RuntimeIdFactory = parentScene.RuntimeIdFactory;
            scene.Id = scene.EntityIdFactory.Create;
            scene.RunTimeId = scene.RuntimeIdFactory.Create;
            scene.AddEntity(scene);
            await SetScheduler(scene, parentScene, SceneRuntimeType.ThreadPool);
            
            Process.AddScene(scene);
            parentScene.Process.AddSceneToProcess(scene);
            
            scene.ThreadSynchronizationContext.Post(() => OnEvent().Coroutine());
            return scene;
            async FTask OnEvent()
            {
                await scene.EventComponent.PublishAsync(new OnCreateScene(scene));
                onSubSceneComplete?.Invoke(scene, parentScene);
            }
        }
#endif
        private static async FTask SetScheduler(Scene scene, Scene schedulerScene, string sceneRuntimeType)
        {
            switch (sceneRuntimeType)
            {
                case "MainThread":
                {
                    scene.ThreadSynchronizationContext = ThreadScheduler.MainScheduler.ThreadSynchronizationContext;
                    scene.SceneUpdate = new EmptySceneUpdate();
                    ThreadScheduler.AddMainScheduler(scene);
                    await scene.Initialize();
                    break;
                }
                case "MultiThread":
                {
#if !FANTASY_WEBGL
                    scene.ThreadSynchronizationContext = new ThreadSynchronizationContext();
#endif
                    scene.SceneUpdate = new EmptySceneUpdate();
                    ThreadScheduler.AddToMultiThreadScheduler(scene);
                    await scene.Initialize();
                    break;
                }
                case "ThreadPool":
                {
#if !FANTASY_WEBGL
                    scene.ThreadSynchronizationContext = new ThreadSynchronizationContext();   
#endif
                    scene.SceneUpdate = new EmptySceneUpdate();
                    ThreadScheduler.AddToThreadPoolScheduler(scene);
                    await scene.Initialize();
                    break;
                }
                case "SceneThread":
                {
                    scene.ThreadSynchronizationContext = schedulerScene.ThreadSynchronizationContext;
                    scene.Initialize(schedulerScene);
                    break;
                }
            }
        }
        #endregion

        #region Entities

        /// <summary>
        /// 添加一个实体到当前Scene下
        /// </summary>
        /// <param name="entity">实体实例</param>
        public void AddEntity(Entity entity)
        {
            _entities.Add(entity.RunTimeId, entity);
        }

        /// <summary>
        /// 根据RunTimeId查询一个实体
        /// </summary>
        /// <param name="runTimeId">实体的RunTimeId</param>
        /// <returns>返回的实体</returns>
        public Entity GetEntity(long runTimeId)
        {
            return _entities.TryGetValue(runTimeId, out var entity) ? entity : null;
        }

        /// <summary>
        /// 根据RunTimeId查询一个实体
        /// </summary>
        /// <param name="runTimeId">实体的RunTimeId</param>
        /// <param name="entity">实体实例</param>
        /// <returns>返回一个bool值来提示是否查找到这个实体</returns>
        public bool TryGetEntity(long runTimeId, out Entity entity)
        {
            return _entities.TryGetValue(runTimeId, out entity);
        }

        /// <summary>
        /// 根据RunTimeId查询一个实体
        /// </summary>
        /// <param name="runTimeId">实体的RunTimeId</param>
        /// <typeparam name="T">要查询实体的泛型类型</typeparam>
        /// <returns>返回的实体</returns>
        public T GetEntity<T>(long runTimeId) where T : Entity
        {
            return _entities.TryGetValue(runTimeId, out var entity) ? (T)entity : null;
        }

        /// <summary>
        /// 根据RunTimeId查询一个实体
        /// </summary>
        /// <param name="runTimeId">实体的RunTimeId</param>
        /// <param name="entity">实体实例</param>
        /// <typeparam name="T">要查询实体的泛型类型</typeparam>
        /// <returns>返回一个bool值来提示是否查找到这个实体</returns>
        public bool TryGetEntity<T>(long runTimeId, out T entity) where T : Entity
        {
            if (_entities.TryGetValue(runTimeId, out var getEntity))
            {
                entity = (T)getEntity;
                return true;
            }

            entity = null;
            return false;
        }

        /// <summary>
        /// 删除一个实体，仅是删除不会指定实体的销毁方法
        /// </summary>
        /// <param name="runTimeId">实体的RunTimeId</param>
        /// <returns>返回一个bool值来提示是否删除了这个实体</returns>
        public bool RemoveEntity(long runTimeId)
        {
            return _entities.Remove(runTimeId);
        }

        /// <summary>
        /// 删除一个实体，仅是删除不会指定实体的销毁方法
        /// </summary>
        /// <param name="entity">实体实例</param>
        /// <returns>返回一个bool值来提示是否删除了这个实体</returns>
        public bool RemoveEntity(Entity entity)
        {
            return _entities.Remove(entity.RunTimeId);
        }

        #endregion

        #region InnerSession

#if FANTASY_NET
        /// <summary>
        /// 根据runTimeId获得Session
        /// </summary>
        /// <param name="runTimeId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public Session GetSession(long runTimeId)
        {
            var sceneId = RuntimeIdFactory.GetSceneId(ref runTimeId);

            if (_processSessionInfos.TryGetValue(sceneId, out var processSessionInfo))
            {
                if (!processSessionInfo.Session.IsDisposed)
                {
                    return processSessionInfo.Session;
                }

                _processSessionInfos.Remove(sceneId);
            }

            if (Process.IsInAppliaction(ref sceneId))
            {
                // 如果在同一个Process下，不需要通过Socket发送了，直接通过Process下转发。
                var processSession = Session.CreateInnerSession(Scene);
                _processSessionInfos.Add(sceneId, new ProcessSessionInfo(processSession, null));
                return processSession;
            }

            if (!SceneConfigData.Instance.TryGet(sceneId, out var sceneConfig))
            {
                throw new Exception($"The scene with sceneId {sceneId} was not found in the configuration file");
            }

            if (!ProcessConfigData.Instance.TryGet(sceneConfig.ProcessConfigId, out var processConfig))
            {
                throw new Exception($"The process with processId {sceneConfig.ProcessConfigId} was not found in the configuration file");
            }

            if (!MachineConfigData.Instance.TryGet(processConfig.MachineId, out var machineConfig))
            {
                throw new Exception($"The machine with machineId {processConfig.MachineId} was not found in the configuration file");
            }
            
            var remoteAddress = $"{machineConfig.InnerBindIP}:{sceneConfig.InnerPort}";
            var client = NetworkProtocolFactory.CreateClient(Scene, ProcessDefine.InnerNetwork, NetworkTarget.Inner);
            var session = client.Connect(remoteAddress, null, () =>
            {
                Log.Error($"Unable to connect to the target server sourceServerId:{Scene.Process.Id} targetServerId:{sceneConfig.ProcessConfigId}");
            }, null, false);
            _processSessionInfos.Add(sceneId, new ProcessSessionInfo(session, client));
            return session;
        }
#endif
        #endregion
    }
}