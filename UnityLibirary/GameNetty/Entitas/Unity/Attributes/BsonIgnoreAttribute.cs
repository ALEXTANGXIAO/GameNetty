#if GAME_UNITY
using System;
namespace MongoDB.Bson.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class BsonIgnoreAttribute : Attribute
    {
    
    }
}
#endif
