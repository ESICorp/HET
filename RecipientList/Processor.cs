using Het.Backend;
using Het.Common;
using System;
using System.Threading;

namespace RecipientList
{
    [Component]
    public partial class Processor
    {
        [Autowired]
        public Application Application { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            var recipientLists = AssemblyHelper.GetComponents<RecipientListAttribute>(this.Application.ExternalComponents);

            foreach (var recipientList in recipientLists)
            {
                ThreadPool.QueueUserWorkItem(this.ProcessQueue, recipientList);
            }
        }
    }
}
