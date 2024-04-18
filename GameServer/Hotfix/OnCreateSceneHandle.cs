using GameServer;

/// <summary>
/// Scene创建后处理。
/// </summary>
public class OnCreateSceneHandle : AsyncEventSystem<OnCreateSceneEvent>
{
    public override async FTask Handler(OnCreateSceneEvent self)
    {
        // GameServer服务器是以Scene为单位的、所以Scene下有什么组件都可以自己添加定义
        // OnCreateScene这个事件就是给开发者使用的
        // 比如Address协议这里、我就是做了一个管理Address地址的一个组件挂在到Address这个Scene下面了
        // 比如Map下你需要一些自定义组件、你也可以在这里操作
        var scene = self.Scene;
        switch (scene.SceneType)
        {
            case SceneType.Addressable:
            {
                // 挂载管理Address地址组件
                scene.AddComponent<AddressableManageComponent>();
                break;
            }
            case SceneType.Gate:
            {
                break;
            }
            case SceneType.Map:
            {
                // 挂UnitComponent组件，存取unit
                // scene.AddComponent<UnitComponent>();
                break;
            }
        }
        
        Log.Info($"scene create: {self.Scene.SceneType} SceneId:{self.Scene.Id} LocationId:{self.Scene.LocationId} WorldId:{self.Scene.World?.Id}");


        await FTask.CompletedTask;
    }
}