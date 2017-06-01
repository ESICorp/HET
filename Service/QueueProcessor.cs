using Het.Backend;
using Het.Common;
using System;
using System.Diagnostics;
using System.Threading;

namespace Service
{
    public partial class Processor 
    {
        private void ProcessQueue(object @object)
        {
            var tuple = AssemblyHelper.GetMethodInfoAttribute<ServiceAttribute>(@object);
            var methodInfo = tuple.Item1;
            var attribute = tuple.Item2;

            while (this.Application.Active)
            {
                try
                {
                    string id = null;

                    var command = MessageHelper.Receive(attribute.InputChannel, TimeSpan.FromSeconds(10), out id) as Command;

                    if (command != null && id != null)
                    {
                        command.Context = new Context()
                        {
                            Id = id,
                            Component = @object,
                            Attribute = attribute,
                            MethodInfo = methodInfo
                        };

                        ThreadPool.QueueUserWorkItem(this.ProcessRequest, command);
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError("Couldn't receive {0}: {1}", attribute.InputChannel, e.Message);
                }
            }

            Trace.TraceWarning("End Service");
        }
    }
}
