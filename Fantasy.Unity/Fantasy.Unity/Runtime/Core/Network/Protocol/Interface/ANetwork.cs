using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Fantasy.Entitas;
using Fantasy.PacketParser;
using Fantasy.Scheduler;
using Fantasy.Serialize;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Fantasy.Network.Interface
{
    /// <summary>
    /// 抽象网络基类。
    /// </summary>
    public abstract class ANetwork : Entity
    {
        private const int MaxMemoryStreamSize = 2048;
        
        private long _outerPackInfoId;
        private Queue<OuterPackInfo> _outerPackInfoPool;
        private readonly Queue<MemoryStreamBuffer> _memoryStreamPool = new Queue<MemoryStreamBuffer>();
        
        public NetworkType NetworkType { get; private set; }
        public NetworkTarget NetworkTarget { get; private set; }
        public NetworkProtocolType NetworkProtocolType { get; private set; }
        public ANetworkMessageScheduler NetworkMessageScheduler { get; private set; }

        protected void Initialize(NetworkType networkType, NetworkProtocolType networkProtocolType, NetworkTarget networkTarget)
        {
            NetworkType = networkType;
            NetworkTarget = networkTarget;
            NetworkProtocolType = networkProtocolType;
#if FANTASY_NET
            if (networkProtocolType == NetworkProtocolType.HTTP)
            {
                return;
            }
            if (networkTarget == NetworkTarget.Inner)
            {
                _innerPackInfoPool = new Queue<InnerPackInfo>();
                NetworkMessageScheduler = new InnerMessageScheduler(Scene);
                return;
            }
#endif         
            switch (networkType)
            {
                case NetworkType.Client:
                {
                    _outerPackInfoPool = new Queue<OuterPackInfo>();
                    NetworkMessageScheduler = new ClientMessageScheduler(Scene);
                    break;
                }
#if FANTASY_NET
                case NetworkType.Server:
                {
                    _outerPackInfoPool = new Queue<OuterPackInfo>();
                    NetworkMessageScheduler = new OuterMessageScheduler(Scene);
                    break;
                }
#endif
            }
        }
        
        public abstract void RemoveChannel(uint channelId);

        public MemoryStreamBuffer RentMemoryStream(int size = 0)
        {
            if (size > MaxMemoryStreamSize)
            {
                return new MemoryStreamBuffer(size);
            }

            if (size < MaxMemoryStreamSize)
            {
                size = MaxMemoryStreamSize;
            }

            if (_memoryStreamPool.Count == 0)
            {
                return new MemoryStreamBuffer(size);
            }

            if (_memoryStreamPool.TryDequeue(out var memoryStream))
            {
                return memoryStream;
            }

            return new MemoryStreamBuffer(size);
        }

        public void ReturnMemoryStream(MemoryStreamBuffer memoryStreamBuffer)
        {
            if (memoryStreamBuffer.Capacity > MaxMemoryStreamSize)
            {
                return;
            }
            
            if (_memoryStreamPool.Count > 512)
            {
                // 设置该值只能是内网或服务器转发的时候可能在连接之前发送的数据过多的情况下可以修改。
                // 设置过大会导致内存占用过大，所以要谨慎设置。
                return;
            }
            
            memoryStreamBuffer.SetLength(0);
            _memoryStreamPool.Enqueue(memoryStreamBuffer);
        }
        
        public OuterPackInfo RentOuterPackInfo()
        {
            if (_outerPackInfoPool.Count == 0)
            {
                return new OuterPackInfo()
                {
                    PackInfoId = ++_outerPackInfoId
                };
            }

            if (!_outerPackInfoPool.TryDequeue(out var outerPackInfo))
            {
                return new OuterPackInfo()
                {
                    PackInfoId = ++_outerPackInfoId
                };
            }
            
            outerPackInfo.PackInfoId = ++_outerPackInfoId;
            return outerPackInfo;
        }

        public void ReturnOuterPackInfo(OuterPackInfo outerPackInfo)
        {
            if (_outerPackInfoPool.Count > 512)
            {
                // 池子里最多缓存256个、其实这样设置有点多了、其实用不了512个。
                // 反而设置越大内存会占用越多。
                return;
            }
            
            _outerPackInfoPool.Enqueue(outerPackInfo);
        }
#if FANTASY_NET
        private long _innerPackInfoId;
        private Queue<InnerPackInfo> _innerPackInfoPool;
        public InnerPackInfo RentInnerPackInfo()
        {
            if (_innerPackInfoPool.Count == 0)
            {
                return new InnerPackInfo()
                {
                    PackInfoId = ++_innerPackInfoId
                };
            }

            if (!_innerPackInfoPool.TryDequeue(out var innerPackInfo))
            {
                return new InnerPackInfo()
                {
                    PackInfoId = ++_innerPackInfoId
                };
            }
                
            innerPackInfo.PackInfoId = ++_innerPackInfoId;
            return innerPackInfo;
        }

        public void ReturnInnerPackInfo(InnerPackInfo innerPackInfo)
        {
            if (_innerPackInfoPool.Count > 256)
            {
                // 池子里最多缓存256个、其实这样设置有点多了、其实用不了256个。
                // 反而设置越大内存会占用越多。
                return;
            }
            
            _innerPackInfoPool.Enqueue(innerPackInfo);
        }
#endif 
        public override void Dispose()
        {
            NetworkType = NetworkType.None;
            NetworkTarget = NetworkTarget.None;
            NetworkProtocolType = NetworkProtocolType.None;
            foreach (var memoryStream in _memoryStreamPool)
            {
                memoryStream.Dispose();
            }
            _memoryStreamPool.Clear();
            _outerPackInfoPool?.Clear();
#if FANTASY_NET
            _innerPackInfoPool?.Clear();
#endif
            base.Dispose();
        }
    }
}