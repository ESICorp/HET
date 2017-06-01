using Het.Common;
using System.Diagnostics;

namespace Dummy
{
    [Component]
    public class Class2
    {
        [Splitter("cola2", "cola3")]
        public string[] Run([Request] Request request)
        {
            Trace.TraceInformation("Class2.Run");

            return request.Message.Split('|');
        }
    }
}
