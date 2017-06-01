using Het.Common;
using System.Diagnostics;

namespace Dummy
{
    [Component]
    public class Class1
    {
        [Router("cola1")]
        public string Run()
        {
            Trace.TraceInformation("Class1.Run");

            return "cola2";
        }
    }
}
