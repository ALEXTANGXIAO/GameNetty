using System.Collections.Generic;

namespace ET
{
    [EnableClass]
    public class HttpGetRouterResponse
    {
        public List<string> Realms { get; set; } = new();

        public List<string> Routers { get; set; } = new();
        
        public static HttpGetRouterResponse Create(bool isFromPool = false)
        {
            return ObjectPool.Instance.Fetch(typeof(HttpGetRouterResponse), isFromPool) as HttpGetRouterResponse;
        }
    }
}