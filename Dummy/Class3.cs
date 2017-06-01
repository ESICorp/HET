using Het.Common;
using System.Diagnostics;

namespace Dummy
{
    [Component]
    public class Class3
    {
        [Service("cola3", "cola4")]
        public void Run([Command] Command command)
        {
            Trace.TraceInformation("Class3.Run");

            if (command.Request.Message == "a") command.Response.Message = "1";
            else if (command.Request.Message == "b") command.Response.Message = "2";
            else if (command.Request.Message == "c") command.Response.Message = "3";
            else if (command.Request.Message == "d") command.Response.Message = "4";
            else if (command.Request.Message == "e") command.Response.Message = "5";
            else if (command.Request.Message == "f") command.Response.Message = "6";
            else command.Response.Message = "0";
        }
    }
}
