using System;

namespace GameServer
{
    /// <summary>
    /// 提供用于生成不同类型 ID 的工厂类。
    /// </summary>
    public static class IdFactory
    {
        // 时间戳计算相关常量
        private static readonly long Epoch1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
        private static readonly long Epoch2023 = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000 - Epoch1970;
        private static readonly long EpochThisYear = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000 - Epoch1970;
        // 运行时 ID 相关字段
        private static uint _lastRunTimeIdTime;
        private static uint _lastRunTimeIdSequence;
        // 实体 ID 相关字段
        private static uint _lastEntityIdTime;
        private static uint _lastEntityIdSequence;

        /// <summary>
        /// 生成下一个运行时 ID。
        /// </summary>
        /// <returns>生成的运行时 ID。</returns>
        public static long NextRunTimeId()
        {
            var time = (uint) ((TimeHelper.Now - EpochThisYear) / 1000);

            if (time > _lastRunTimeIdTime)
            {
                _lastRunTimeIdTime = time;
                _lastRunTimeIdSequence = 0;
            }
            else if (++_lastRunTimeIdSequence > uint.MaxValue - 1)
            {
                ++_lastRunTimeIdTime;
                _lastRunTimeIdSequence = 0;
            }

            return new RuntimeIdStruct(_lastRunTimeIdTime, _lastRunTimeIdSequence);
        }

        /// <summary>
        /// 生成下一个实体 ID。
        /// </summary>
        /// <param name="locationId">位置 ID。</param>
        /// <returns>生成的实体 ID。</returns>
        public static long NextEntityId(uint locationId)
        {
            var time = (uint)((TimeHelper.Now - Epoch2023) / 1000);

            if (time > _lastEntityIdTime)
            {
                _lastEntityIdTime = time;
                _lastEntityIdSequence = 0;
            }
            else if (++_lastEntityIdSequence > EntityIdStruct.MaskSequence - 1)
            {
                ++_lastEntityIdTime;
                _lastEntityIdSequence = 0;
            }

            return new EntityIdStruct(locationId, _lastEntityIdTime, _lastEntityIdSequence);
        }

        /// <summary>
        /// 获取实体 ID 对应的路由 ID。
        /// </summary>
        /// <param name="entityId">实体 ID。</param>
        /// <returns>路由 ID。</returns>
        public static uint GetRouteId(long entityId)
        {
            return (ushort)(entityId >> 16 & EntityIdStruct.MaskRouteId);
        }

        /// <summary>
        /// 获取实体 ID 对应的应用 ID。
        /// </summary>
        /// <param name="entityId">实体 ID。</param>
        /// <returns>应用 ID。</returns>
        public static ushort GetAppId(long entityId)
        {
            return (ushort)(entityId >> 26 & RouteIdStruct.MaskAppId);
        }

        /// <summary>
        /// 获取实体 ID 对应的世界 ID。
        /// </summary>
        /// <param name="entityId">实体 ID。</param>
        /// <returns>世界 ID。</returns>
        public static int GetWordId(long entityId)
        {
            return (ushort)(entityId >> 16 & RouteIdStruct.MaskWordId);
        }
    }
}

