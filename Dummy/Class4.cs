using Het.Common;
using System.Diagnostics;

namespace Dummy
{
    [Component]
    public class Class4
    {
        [Aggregator("cola4", "cola5")]
        public void Run([Command] Command command)
        {
            Trace.TraceInformation("Class4.Run");

            command.Response.Message = string.Empty;

            var aux = new string[command.Length];

            for (int i=0; i<command.Length; i++) {

                aux[i] = command.Partial[i].Message;
            }

            command.Response.Message = string.Join(";", aux);
        }
    }
}
