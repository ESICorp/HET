using Het.Backend;
using Het.Common;
using System;
using System.Threading;

namespace Service
{
    [Component]
    public partial class Processor 
    {
        [Autowired]
        public Application Application { get; set; }

        private int Version { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            var services = AssemblyHelper.GetComponents<ServiceAttribute>(this.Application.ExternalComponents);

            foreach(var service in services)
            {
                ThreadPool.QueueUserWorkItem(this.ProcessQueue, service);
            }
        }
    }
}
