using Het.Backend;
using Het.Common;
using System;
using System.Threading;

namespace Aggregator
{
    [Component]
    public partial class Processor
    {
        [Autowired]
        public Application Application { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            var aggregators = AssemblyHelper.GetComponents<AggregatorAttribute>(Application.ExternalComponents);

            foreach (var aggregator in aggregators)
            {
                ThreadPool.QueueUserWorkItem(this.ProcessQueue, aggregator);
            }
        }
    }
}
