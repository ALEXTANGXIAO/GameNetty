using UnityEditor;
using UnityEngine;

namespace ET
{
    public static class ToolsEditor
    {
        [MenuItem("ET/Luban 转表 Client")]
        public static void BuildLubanExcelClient()
        { 
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            Application.OpenURL(Application.dataPath + @"/../../Tools/Luban/GenConfig_Client.sh");
#else
            Application.OpenURL(Application.dataPath + @"/../../Tools/Luban/GenConfig_Client.bat");
#endif
        }
        
        [MenuItem("ET/Luban 转表 Server")]
        public static void BuildLubanExcelServer()
        {
            Application.OpenURL(Application.dataPath + @"/../../Tools/Luban/GenConfig_Server.bat");
        }

        private const string clientMessagePath = "../Unity/Assets/GameScripts/GameProto/Generate/Message/";
        
        private const string serverMessagePath = "../Server/Model/Generate/Message/";

        private const string protoDir = "../Config/Proto";
        
        [MenuItem("ET/Build Proto2CS")]
        public static void Proto2CS()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            // ShellHelper.Run($"{tools} --AppType=Proto2CS --Console=1 {protoDir} {clientMessagePath} {serverMessagePath}", "../Bin/");
            ShellHelper.Run($"{tools} --AppType=Proto2CS --Console=1", "../Bin/");
        }
    }
}