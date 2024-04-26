using ET;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using UnityEngine;

public class Init : MonoBehaviour
{
    [MessageHandler(SceneType.NetClient)]
    public class Main2NetClient_LoginHandler : MessageHandler<Scene, Main2NetClient_Login, NetClient2Main_Login>
    {
        protected override async ETTask Run(Scene root, Main2NetClient_Login request, NetClient2Main_Login response)
        {
            string account = request.Account;
            string password = request.Password;

            // 创建一个ETModel层的Session
            root.RemoveComponent<RouterAddressComponent>();
            // 获取路由跟realmDispatcher地址
            RouterAddressComponent routerAddressComponent =
                root.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
            await routerAddressComponent.Init();
            root.AddComponent<NetComponent, AddressFamily, NetworkProtocol>(
                routerAddressComponent.RouterManagerIPAddress.AddressFamily, 
                NetworkProtocol.UDP);
            root.GetComponent<FiberParentComponent>().ParentFiberId = request.OwnerFiberId;

            NetComponent netComponent = root.GetComponent<NetComponent>();
            
            IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

            R2C_Login r2CLogin;
            using (Session session = await netComponent.CreateRouterSession(realmAddress, account, password))
            {
                C2R_Login c2RLogin = C2R_Login.Create();
                c2RLogin.Account = account;
                c2RLogin.Password = password;
                r2CLogin = (R2C_Login)await session.Call(c2RLogin);
            }

            // 创建一个gate Session,并且保存到SessionComponent中
            Session gateSession = await netComponent.CreateRouterSession(NetworkHelper.ToIPEndPoint(r2CLogin.Address), account, password);
            gateSession.AddComponent<ClientSessionErrorComponent>();
            root.AddComponent<SessionComponent>().Session = gateSession;
            C2G_LoginGate c2GLoginGate = C2G_LoginGate.Create();
            c2GLoginGate.Key = r2CLogin.Key;
            c2GLoginGate.GateId = r2CLogin.GateId;
            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(c2GLoginGate);

            Log.Debug("登陆gate成功!");

            response.PlayerId = g2CLoginGate.PlayerId;
            
            Log.Debug("登陆gate成功!");
            await ETTask.CompletedTask;
        }
    }

    [Event(SceneType.Main)]
    public class OnAppStartInitFinish : AEvent<Scene, AppStartInitFinish>
    {
        protected override async ETTask Run(Scene root, AppStartInitFinish args)
        {
            await ETTask.CompletedTask;

            Log.Warning("On AppStartInit Finish");

            LoginAsync(root, "123", "123").Coroutine();
        }

        public static async ETTask<long> LoginAsync(Scene root, string account, string password)
        {
            var fiberId = await FiberManager.Instance.Create(SchedulerType.ThreadPool, 0, SceneType.NetClient, "");
            var netClientActorId = new ActorId(root.Fiber().Process, fiberId);

            Main2NetClient_Login main2NetClientLogin = Main2NetClient_Login.Create();
            main2NetClientLogin.OwnerFiberId = root.Fiber().Id;
            main2NetClientLogin.Account = account;
            main2NetClientLogin.Password = password;
            NetClient2Main_Login response =
                await root.Root().GetComponent<ProcessInnerSender>().Call(netClientActorId, main2NetClientLogin) as NetClient2Main_Login;

            if (response != null && response.Error != 0)
            {
            }

            // return response.PlayerId;
            return 0;
        }
    }

    private void Start()
    {
        this.StartAsync().Coroutine();
    }

    private async ETTask StartAsync()
    {
        DontDestroyOnLoad(gameObject);

        AppDomain.CurrentDomain.UnhandledException += (sender, e) => { Log.Error(e.ExceptionObject.ToString()); };

        World.Instance.AddSingleton<ET.Logger>().Log = new UnityLogger();
        ETTask.ExceptionHandler += Log.Error;

        World.Instance.AddSingleton<TimeInfo>();
        World.Instance.AddSingleton<FiberManager>();

        // CodeLoader codeLoader = World.Instance.AddSingleton<CodeLoader>();
        // await codeLoader.DownloadAsync();
        //
        // codeLoader.Start();
        await ETTask.CompletedTask;

        Log.Debug($"StartAsync");

        Assembly runtime = typeof(Entry).Assembly;

        World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[]
        {
            typeof(World).Assembly, typeof(Init).Assembly,
            typeof(C2G_Ping).Assembly,typeof(PingComponent).Assembly
        });

        IStaticMethod start = new StaticMethod(runtime, "ET.Entry", "Start");
        start.Run();
    }

    private void Update()
    {
        TimeInfo.Instance.Update();
        FiberManager.Instance.Update();
    }

    private void LateUpdate()
    {
        FiberManager.Instance.LateUpdate();
    }

    private void OnApplicationQuit()
    {
        World.Instance.Dispose();
    }
}