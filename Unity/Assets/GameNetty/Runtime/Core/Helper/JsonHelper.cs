using System;
using Newtonsoft.Json;

namespace ET
{
    public static class JsonHelper
    {
        public static string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T ToObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object ToObject(Type objectType, string json)
        {
            return JsonConvert.DeserializeObject(json);
        }
    }
}