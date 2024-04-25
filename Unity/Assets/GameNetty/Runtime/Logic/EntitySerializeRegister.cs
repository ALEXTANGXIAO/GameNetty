namespace ET
{
    // TODO 这里有个SourceGenerate
    public static partial class EntitySerializeRegister
    {
        static partial void Register();

        public static void Init()
        {
            Register();
        }
    }
}

