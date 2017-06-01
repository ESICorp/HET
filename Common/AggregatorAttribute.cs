using System;

namespace Het.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AggregatorAttribute : Attribute
    {
        public string InputChannel { get; set; }
        public string OutputChannel { get; set; }
        public string Deadletter { get; set; }

        public AggregatorAttribute(string inputChannel, string ouputChannel)
        {
            this.InputChannel = inputChannel;
            this.OutputChannel = ouputChannel;
        }

        public AggregatorAttribute(string inputChannel, string outputChannel, string deadletter)
        {
            this.InputChannel = inputChannel;
            this.OutputChannel = outputChannel;
            this.Deadletter = deadletter;
        }
    }
}
