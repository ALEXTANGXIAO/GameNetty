using System;
#if GAME_UNITY
using UnityEngine;
#endif

namespace GameNetty
{
    /// <summary>
    /// 提供与时间相关的帮助方法。
    /// </summary>
    public static class TimeHelper
    {
        /// <summary>
        /// 一小时的毫秒值。
        /// </summary>
        public const long Hour = 3600000;

        /// <summary>
        /// 一分钟的毫秒值。
        /// </summary>
        public const long Minute = 60000;

        /// <summary>
        /// 一天的毫秒值。
        /// </summary>
        public const long OneDay = 86400000;

        // 1970年1月1日的Ticks
        private const long Epoch = 621355968000000000L;
        private static readonly DateTime Dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取当前时间的毫秒数，从1970年1月1日开始计算。
        /// </summary>
        public static long Now => (DateTime.UtcNow.Ticks - Epoch) / 10000;

        /// <summary>
        /// 获取帧运行毫秒数。
        /// </summary>
        public static long TotalFrameTime;

        /// <summary>
        /// 获取当前帧与上一帧的时间间隔毫秒数。
        /// </summary>
        public static int FrameDeltaTime;

        /// <summary>
        /// 获取当前帧与上一帧的时间间隔秒数
        /// </summary>
        public static float FrameDeltaTimeSeconds => FrameDeltaTime / 1000f;

        /// <summary>
        /// 记录上一帧的总时间。
        /// </summary>
        public static long PreviousFrameTime;
#if GAME_UNITY
        /// <summary>
        /// 与服务器时间的偏差。
        /// </summary>
        public static long TimeDiff;
        /// <summary>
        /// 获取当前服务器时间的毫秒数，加上与服务器时间的偏差。
        /// </summary>
        public static long ServerNow => Now + TimeDiff;
        /// <summary>
        /// 获取当前Unity运行的总时间的毫秒数。
        /// </summary>
        public static long UnityNow => (long) (Time.time * 1000);
#endif
        /// <summary>
        /// 将日期时间转换为毫秒数，从1970年1月1日开始计算。
        /// </summary>
        /// <param name="d">要转换的日期时间。</param>
        /// <returns>转换后的毫秒数。</returns>
        public static long Transition(DateTime d)
        {
            return (d.Ticks - Epoch) / 10000;
        }

        /// <summary>
        /// 将毫秒数转换为日期时间。
        /// </summary>
        /// <param name="timeStamp">要转换的毫秒数。</param>
        /// <returns>转换后的日期时间。</returns>
        public static DateTime Transition(long timeStamp)
        {
            return Dt1970.AddTicks(timeStamp);
        }

        /// <summary>
        /// 将毫秒数转换为本地时间的日期时间。
        /// </summary>
        /// <param name="timeStamp">要转换的毫秒数。</param>
        /// <returns>转换后的本地时间的日期时间。</returns>
        public static DateTime TransitionLocal(long timeStamp)
        {
            return Dt1970.AddTicks(timeStamp).ToLocalTime();
        }
    }
}