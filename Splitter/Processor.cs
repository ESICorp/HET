using Het.Backend;
using Het.Common;
using System;
using System.Threading;

namespace Splitter
{
    [Component]
    public partial class Processor
    {
        [Autowired]
        public Application Application { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            var splitters = AssemblyHelper.GetComponents<SplitterAttribute>(this.Application.ExternalComponents);

            foreach (var splitter in splitters)
            {
                ThreadPool.QueueUserWorkItem(this.ProcessQueue, splitter);
            }
        }
    }
}
