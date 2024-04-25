using System.Reflection;

#pragma warning disable CS8603

namespace GameNetty;

/// <summary>
/// 整个框架使用的程序集、有几个程序集就定义集。这里定义是为了后面方面使用
/// </summary>
public static class AssemblyName
{
    public const int Hotfix = 1;
}

public static class AssemblyHelper
{
    public static void Initialize()
    {
        LoadHotfix();
    }

    public static void LoadHotfix()
    {
        CodeTypes.Instance.Init(new []{typeof(AssemblyHelper).Assembly});
        AssemblyManager.Load(AssemblyName.Hotfix, typeof(AssemblyHelper).Assembly);
    }

    public static Dictionary<string, Type> GetAssemblyTypes(params Assembly[] args)
    {
        Dictionary<string, Type> types = new Dictionary<string, Type>();

        foreach (Assembly ass in args)
        {
            foreach (Type type in ass.GetTypes())
            {
                if (type.FullName != null)
                {
                    types[type.FullName] = type;
                }
            }
        }

        return types;
    }
}