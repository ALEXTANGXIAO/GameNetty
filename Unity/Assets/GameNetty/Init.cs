using ET;
using System;
using System.Reflection;
using UnityEngine;

public class Init : MonoBehaviour
{
    [Event(SceneType.Main)]
    public class OnAppStartInitFinish: AEvent<Scene, AppStartInitFinish>
    {
        protected override async ETTask Run(Scene root, AppStartInitFinish args)
        {
            await ETTask.CompletedTask;
            
            Log.Warning("On AppStartInit Finish");
            

        }
        
        public static async ETTask<long> LoginAsync(Scene root, string account, string password)
        {
            var fiberId = await FiberManager.Instance.Create(SchedulerType.ThreadPool, 0, SceneType.NetClient, "");
            var netClientActorId = new ActorId(root.Fiber().Process, fiberId);

            // Main2NetClient_Login main2NetClientLogin = Main2NetClient_Login.Create();
            // main2NetClientLogin.OwnerFiberId = root.Fiber().Id;
            // main2NetClientLogin.Account = account;
            // main2NetClientLogin.Password = password;
            // NetClient2Main_Login response = await root.Root().GetComponent<ProcessInnerSender>().Call(netClientActorId, main2NetClientLogin) as NetClient2Main_Login;
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
        
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Log.Error(e.ExceptionObject.ToString());
        };
        
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
            typeof (World).Assembly, typeof (Init).Assembly,
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
