using ET;
using System;
using System.Reflection;
using UnityEngine;

public class Init : MonoBehaviour
{
    public static Scene Root { private set; get; }
    
    [Event(SceneType.Main)]
    public class OnAppStartInitFinish : AEvent<Scene, AppStartInitFinish>
    {
        protected override async ETTask Run(Scene root, AppStartInitFinish args)
        {
            await ETTask.CompletedTask;

            Log.Warning("On AppStartInit Finish");

            Root = root;
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

        // 注册Entity序列化器
        EntitySerializeRegister.Init();
        
        World.Instance.AddSingleton<ET.Logger>().Log = new UnityLogger();
        ETTask.ExceptionHandler += Log.Error;

        World.Instance.AddSingleton<TimeInfo>();
        World.Instance.AddSingleton<FiberManager>();

        await ETTask.CompletedTask;

        Log.Debug($"StartAsync");

        // GameNetty.Runtime (Assembly)
        Assembly runtime = typeof(Entry).Assembly;

        World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[]
        {
            typeof(Entity).Assembly,
            // GameNetty.Runtime (Assembly)
            typeof(Entry).Assembly,
            // Assembly-CSharp (Assembly)
            typeof(Init).Assembly,
            // GameProto (Assembly)
            typeof(C2G_Ping).Assembly,
            // GameLogic (Assembly)
            typeof(PingComponent).Assembly
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