using UnityEditor;

namespace ET
{
    public static class ToolsEditor
    {
        public static void ExcelExporter()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            const string tools = "./Tool";
#else
            const string tools = ".\\Tool.exe";
#endif
            ShellHelper.Run($"{tools} --AppType=ExcelExporter --Console=1", "../Bin/");
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