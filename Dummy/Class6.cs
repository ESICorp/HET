using Het.Common;
using System.Diagnostics;

namespace Dummy
{
    [Component]
    public class Class6
    {
        [Service("cola6")]
        public void Run()
        {
            Trace.TraceInformation("Class6.Run");

            //do nothing   
        }
    }
}
