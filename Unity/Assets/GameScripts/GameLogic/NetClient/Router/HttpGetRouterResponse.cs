using System.Collections.Generic;

namespace ET
{
    public partial class HttpGetRouterResponse
    {
        public List<string> Realms { get; set; } = new();

        public List<string> Routers { get; set; } = new();
    }
}