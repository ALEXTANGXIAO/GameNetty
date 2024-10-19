using System;
using System.Diagnostics;

namespace ET
{
    public static class Log
    {
        private static ILog GetLog()
        {
            return Fiber.Instance != null? Fiber.Instance.Log : Logger.Instance.Log;
        }
        
        [Conditional("DEBUG")]
        public static void Debug(string msg)
        {
            GetLog().Debug(msg);
        }
        
        [Conditional("DEBUG")]
        public static void Trace(string msg)
        {
            StackTrace st = new(1, true);
            GetLog().Trace($"{msg}\n{st}");
        }

        public static void Info(string msg)
        {
            GetLog().Info(msg);
        }

        public static void TraceInfo(string msg)
        {
            StackTrace st = new(1, true);
            GetLog().Trace($"{msg}\n{st}");
        }

        public static void Warning(string msg)
        {
            GetLog().Warning(msg);
        }

        public static void Error(string msg)
        {
            StackTrace st = new(1, true);
            GetLog().Error($"{msg}\n{st}");
        }

        public static void Error(Exception e)
        {
            GetLog().Error(e.ToString());
        }
        
        public static void Console(string msg)
        {
            GetLog().Debug(msg);
        }
    }
}
