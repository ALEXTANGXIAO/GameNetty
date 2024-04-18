#if GAMESERVER_NET
using System;

namespace GameServer
{
    /// <summary>
    /// 表示当创建新场景时引发的事件数据结构。
    /// </summary>
    public struct OnCreateSceneEvent
    {
        /// <summary>
        /// 获取与事件关联的场景实体。
        /// </summary>
        public readonly Scene Scene;

        /// <summary>
        /// 初始化一个新的 OnCreateScene 实例。
        /// </summary>
        /// <param name="scene">与事件关联的场景实体。</param>
        public OnCreateSceneEvent(Scene scene)
        {
            Scene = scene;
        }
    }
}
#endif