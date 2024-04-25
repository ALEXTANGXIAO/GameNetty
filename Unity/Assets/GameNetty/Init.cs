using System;
using System.Collections;
using System.Collections.Generic;
using ET;
using UnityEngine;

public class Init : MonoBehaviour
{
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
        
        Log.Debug($"StartAsync");
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
