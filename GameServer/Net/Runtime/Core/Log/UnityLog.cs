#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
#else
using System;
#endif

#if GAME_UNITY
namespace GameServer
{
    public class UnityLog : ILog
    {
        public void Trace(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Debug(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Info(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        public void Warning(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
        }

        public void Error(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        public void Error(Exception e)
        {
            UnityEngine.Debug.LogException(e);
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
#endif

#if UNITY_EDITOR
namespace GameServer
{
    /// <summary>
    /// 日志重定向相关的实用函数。
    /// </summary>
    internal static class LogRedirection
    {
        [OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            if (line <= 0)
            {
                return false;
            }

            // 获取资源路径
            string assetPath = AssetDatabase.GetAssetPath(instanceID);
            
            // 判断资源类型
            if (!assetPath.EndsWith(".cs"))
            {
                return false;
            }

            bool autoFirstMatch = assetPath.Contains("Log.cs") ||
                                   assetPath.Contains("UnityLog.cs");

            var stackTrace = GetStackTrace();
            if (!string.IsNullOrEmpty(stackTrace))
                                            
            {
                if (!autoFirstMatch)
                {
                    var fullPath = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.LastIndexOf("Assets", StringComparison.Ordinal));
                    fullPath = $"{fullPath}{assetPath}";
                    // 跳转到目标代码的特定行
                    InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                    return true;
                }

                // 使用正则表达式匹配at的哪个脚本的哪一行
                var matches = Regex.Match(stackTrace, @"\(at (.+)\)",
                    RegexOptions.IgnoreCase);
                while (matches.Success)
                {
                    var pathLine = matches.Groups[1].Value;

                    if (!pathLine.Contains("Log.cs") && 
                        !pathLine.Contains("UnityLog.cs"))
                    {
                        var splitIndex = pathLine.LastIndexOf(":", StringComparison.Ordinal);
                        // 脚本路径
                        var path = pathLine.Substring(0, splitIndex);
                        // 行号
                        line = Convert.ToInt32(pathLine.Substring(splitIndex + 1));
                        var fullPath = UnityEngine.Application.dataPath.Substring(0, UnityEngine.Application.dataPath.LastIndexOf("Assets", StringComparison.Ordinal));
                        fullPath = $"{fullPath}{path}";
                        // 跳转到目标代码的特定行
                        InternalEditorUtility.OpenFileAtLineExternal(fullPath.Replace('/', '\\'), line);
                        break;
                    }

                    matches = matches.NextMatch();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取当前日志窗口选中的日志的堆栈信息。
        /// </summary>
        /// <returns>选中日志的堆栈信息实例。</returns>
        private static string GetStackTrace()
        {
            // 通过反射获取ConsoleWindow类
            var consoleWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
            // 获取窗口实例
            var fieldInfo = consoleWindowType.GetField("ms_ConsoleWindow",
                BindingFlags.Static |
                BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                var consoleInstance = fieldInfo.GetValue(null);
                if (consoleInstance != null)
                    if (EditorWindow.focusedWindow == (EditorWindow)consoleInstance)
                    {
                        // 获取m_ActiveText成员
                        fieldInfo = consoleWindowType.GetField("m_ActiveText",
                            BindingFlags.Instance |
                            BindingFlags.NonPublic);
                        // 获取m_ActiveText的值
                        if (fieldInfo != null)
                        {
                            var activeText = fieldInfo.GetValue(consoleInstance).ToString();
                            return activeText;
                        }
                    }
            }

            return null;
        }
    }
}
#endif