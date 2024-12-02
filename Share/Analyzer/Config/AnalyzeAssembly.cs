using System.IO;

namespace ET.Analyzer
{
    public static class AnalyzeAssembly
    {
        public const string DotNetCore = "Core";
        public const string DotNetModel = "Model";
        public const string DotNetHotfix = "Hotfix";

        public const string UnityCore = "GameLogic";
        public const string UnityModel = "GameLogic";
        public const string UnityHotfix = "GameLogic";
        public const string UnityModelView = "Unity.ModelView";
        public const string UnityHotfixView = "Unity.HotfixView";
        
        public const string UnityETRuntime = "ET.Runtime";
        public const string UnityETCore = "ET.Core";
        public const string UnityETNetWork = "ET.Network";

        public static readonly string[] AllHotfix =
        {
            DotNetHotfix, UnityHotfix, UnityHotfixView,
        };

        public static readonly string[] AllModel =
        {
            DotNetModel, UnityModel, UnityModelView
        };

        public static readonly string[] AllModelHotfix =
        {
            DotNetModel, DotNetHotfix, 
            UnityModel, UnityHotfix, UnityModelView, UnityHotfixView, 
        };
        
        public static readonly string[] All =
        {
            DotNetCore, DotNetModel, DotNetHotfix, 
            UnityCore, UnityModel, UnityHotfix, UnityModelView, UnityHotfixView, 
            UnityETRuntime, UnityETCore, UnityETNetWork
        };

        public static readonly string[] ServerModelHotfix =
        {
            DotNetModel,DotNetHotfix,
        };
        
        public static readonly string[] AllLogicModel =
        {
            DotNetModel, UnityModel , UnityETRuntime, UnityETCore, UnityETNetWork
        };
    }
    
}