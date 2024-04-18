using System.IO;
#if GAME_UNITY || GAMESERVER_NET || GAME_DEVELOP

#else
using Microsoft.IO;
#endif

namespace GameServer
{
    /// <summary>
    /// 提供获取可回收内存流的帮助方法。
    /// </summary>
    public static class MemoryStreamHelper
    {
        private static readonly RecyclableMemoryStreamManager Manager = new RecyclableMemoryStreamManager();

        /// <summary>
        /// 获取一个可回收内存流实例。
        /// </summary>
        /// <returns>可回收内存流实例。</returns>
        public static MemoryStream GetRecyclableMemoryStream()
        {
            return Manager.GetStream();
        }
    }
}