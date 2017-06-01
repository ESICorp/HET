using Het.Backend;
using Het.Common;
using System;
using System.Threading;

namespace Router
{
    [Component]
    public partial class Processor
    {
        [Autowired]
        public Application Application { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            var routers = AssemblyHelper.GetComponents<RouterAttribute>(this.Application.ExternalComponents);

            foreach (var router in routers)
            {
                ThreadPool.QueueUserWorkItem(this.ProcessQueue, router);
            }
        }
    }
}
