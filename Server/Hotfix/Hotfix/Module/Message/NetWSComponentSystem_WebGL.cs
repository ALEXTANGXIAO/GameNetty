using System.Collections.Generic;
using System.Net;

namespace ET
{
    [EntitySystemOf(typeof(NetWSComponent))]
    [FriendOf(typeof(NetWSComponent))]
    public static partial class NetWSComponentSystem
    {
        [EntitySystem]
        private static void Awake(this NetWSComponent self, IEnumerable<string> prefixs)
        {
            self.AService = new WService(prefixs);
            
            self.AService.AcceptCallback = self.OnAccept;
            self.AService.ReadCallback = self.OnRead;
            self.AService.ErrorCallback = self.OnError;
        }
        
        [EntitySystem]
        private static void Awake(this NetWSComponent self)
        {
            Fiber fiber = self.Fiber();
            self.AService = new WService();
            self.AService.ReadCallback = self.OnRead;
            self.AService.ErrorCallback = self.OnError;
        }
        
        [EntitySystem]
        private static void Update(this NetWSComponent self)
        {
            self.AService.Update();
        }

        [EntitySystem]
        private static void Destroy(this NetWSComponent self)
        {
            self.AService.Dispose();
        }

        private static void OnError(this NetWSComponent self, long channelId, int error)
        {
            Session session = self.GetChild<Session>(channelId);
            if (session == null)
            {
                return;
            }

            session.Error = error;
            session.Dispose();
        }

        // 这个channelId是由CreateAcceptChannelId生成的
        private static void OnAccept(this NetWSComponent self, long channelId, IPEndPoint ipEndPoint)
        {
            Session session = self.AddChildWithId<Session, AService>(channelId, self.AService);

            if (self.IScene.SceneType != SceneType.BenchmarkServer)
            {
                // 挂上这个组件，5秒就会删除session，所以客户端验证完成要删除这个组件。该组件的作用就是防止外挂一直连接不发消息也不进行权限验证
                session.AddComponent<SessionAcceptTimeoutComponent>();
                // 客户端连接，2秒检查一次recv消息，10秒没有消息则断开
                session.AddComponent<SessionIdleCheckerComponent>();
            }
        }
        
        private static void OnRead(this NetWSComponent self, long channelId, MemoryBuffer memoryBuffer)
        {
            Session session = self.GetChild<Session>(channelId);
            if (session == null)
            {
                return;
            }
            session.LastRecvTime = TimeInfo.Instance.ClientNow();

            (ActorId _, object message) = MessageSerializeHelper.ToMessage(self.AService, memoryBuffer);
            
            LogMsg.Instance.Debug(self.Fiber(), message);
            
            EventSystem.Instance.Invoke((long)self.IScene.SceneType, new NetComponentOnRead() {Session = session, Message = message});
        }
        
        public static Session Create(this NetWSComponent self, IPEndPoint ipEndPoint)
        {
            long channelId = NetServices.Instance.CreateConnectChannelId();
            Session session = self.AddChildWithId<Session, AService>(channelId, self.AService);
            if (self.IScene.SceneType != SceneType.BenchmarkClient)
            {
                session.AddComponent<SessionIdleCheckerComponent>();
            }
            self.AService.Create(session.Id, ipEndPoint);

            return session;
        }
    }
}