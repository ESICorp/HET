using Het.Common;
using System.Diagnostics;

namespace Dummy
{
    [Component]
    public class Class5
    {
        [RecipientList("cola5")]
        public string[] Run()
        {
            Trace.TraceInformation("Class5.Run");

            return new[] { "cola6", "cola7" };
        }
    }
}
