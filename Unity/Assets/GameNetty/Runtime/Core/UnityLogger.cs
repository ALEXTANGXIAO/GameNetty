using System;
using System.Text.RegularExpressions;

namespace ET
{
    public class UnityLogger: ILog
    {
        public void Trace(string msg)
        {
            UnityEngine.Debug.Log($"<color=gray><b>[Trace] ► </b></color> - {msg}");
        }

        public void Debug(string msg)
        {
            UnityEngine.Debug.Log($"<color=#gray><b>[Debug] ► </b></color> - {msg}");
        }

        public void Info(string msg)
        {
            UnityEngine.Debug.Log($"<color=gray><b>[Info] ► </b></color> - {msg}");
        }

        public void Warning(string msg)
        {
            UnityEngine.Debug.LogWarning($"<color=#FF9400><b>[WARNING] ► </b></color> - {msg}");
        }

        public void Error(string msg)
        {
#if UNITY_EDITOR
            msg = Msg2LinkStackMsg(msg);
#endif
            UnityEngine.Debug.LogError($"<color=red><b>[ERROR] ► </b></color> - {msg}");
        }
        
        private static string Msg2LinkStackMsg(string msg)
        {
            msg = Regex.Replace(msg,@"at (.*?) in (.*?\.cs):(\w+)", match =>
            {
                string path = match.Groups[2].Value;
                string line = match.Groups[3].Value;
                return $"{match.Groups[1].Value}\n<a href=\"{path}\" line=\"{line}\">{path}:{line}</a>";
            });
            return msg;
        }

        public void Error(Exception e)
        {
            UnityEngine.Debug.LogException(new Exception($"<color=red><b>[ERROR] ► </b></color> - {e}"));
        }

        public void Trace(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(message, args);
        }

        public void Info(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public void Debug(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(message, args);
        }

        public void Error(string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(message, args);
        }
    }
}