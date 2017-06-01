using System;

namespace Het.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SplitterAttribute : Attribute
    {
        public string InputChannel { get; set; }
        public string OutputChannel { get; set; }
        public string Deadletter { get; set; }

        public SplitterAttribute(string inputChannel, string ouputChannel)
        {
            this.InputChannel = inputChannel;
            this.OutputChannel = ouputChannel;
        }

        public SplitterAttribute(string inputChannel, string outputChannel, string deadletter)
        {
            this.InputChannel = inputChannel;
            this.OutputChannel = outputChannel;
            this.Deadletter = deadletter;
        }
    }
}
