#if GAMESERVER_NET
namespace GameNetty;

/// <summary>
/// 声明一个 sealed 类 I_AddressableLockHandler，继承自 RouteRPC 类，并指定泛型参数
/// </summary>
public sealed class I_AddressableLockHandler : RouteRPC<Scene, I_AddressableLock_Request, I_AddressableLock_Response>
{
    /// <summary>
    /// 在收到地址映射锁定请求时执行的逻辑。
    /// </summary>
    /// <param name="scene">当前场景实例。</param>
    /// <param name="request">包含请求信息的 I_AddressableLock_Request 实例。</param>
    /// <param name="response">用于构建响应的 I_AddressableLock_Response 实例。</param>
    /// <param name="reply">执行响应的回调操作。</param>
    protected override async GameTask Run(Scene scene, I_AddressableLock_Request request, I_AddressableLock_Response response, Action reply)
    {
        await scene.GetComponent<AddressableManageComponent>().Lock(request.AddressableId);
    }
}
#endif
