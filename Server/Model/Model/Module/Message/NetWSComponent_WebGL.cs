using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ET
{
    [ComponentOf(typeof(Scene))]
    public class NetWSComponent: Entity, IAwake<IEnumerable<string>>, IAwake, IDestroy, IUpdate
    {
        public AService AService;
    }
}