using System;

namespace Het.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceAttribute : Attribute
    {
        public string InputChannel { get; set; }
        public string OutputChannel { get; set; }
        public string Deadletter { get; set; }

        public ServiceAttribute(string inputChannel)
        {
            this.InputChannel = inputChannel;
        }

        public ServiceAttribute(string inputChannel, string outputChannel)
        {
            this.InputChannel = inputChannel;
            this.OutputChannel = outputChannel;
        }

        public ServiceAttribute(string inputChannel, string outputChannel, string deadletter)
        {
            this.InputChannel = inputChannel;
            this.OutputChannel = outputChannel;
            this.Deadletter = deadletter;
        }
    }
}
