using System.Collections.Generic;
using System.Net;

namespace ET.Server
{
    [Invoke((long)SceneType.Gate)]
    public class FiberInit_Gate: AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();
            root.AddComponent<PlayerComponent>();
            root.AddComponent<GateSessionKeyComponent>();
            root.AddComponent<LocationProxyComponent>();
            root.AddComponent<MessageLocationSenderComponent>();

            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Get((int)root.Id);
            root.AddComponent<NetComponent, IPEndPoint, NetworkProtocol>(startSceneConfig.InnerIPPort, NetworkProtocol.UDP);
            
#if DOTNET_WEBGL
            root.AddComponent<NetWSComponent, IEnumerable<string>>(new[]{$"http://*:{startSceneConfig.Port}/"});
#endif
            await ETTask.CompletedTask;
        }
    }
}