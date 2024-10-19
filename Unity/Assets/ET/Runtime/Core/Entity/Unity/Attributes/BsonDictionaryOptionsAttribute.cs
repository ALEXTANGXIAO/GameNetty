#nullable disable
using System;
using MongoDB.Bson.Serialization.Options;

namespace MongoDB.Bson.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BsonDictionaryOptionsAttribute : Attribute
    {
        private DictionaryRepresentation _representation = DictionaryRepresentation.Document;

        public BsonDictionaryOptionsAttribute()
        {
        }

        public BsonDictionaryOptionsAttribute(DictionaryRepresentation representation)
        {
            this._representation = representation;
        }
    }
}