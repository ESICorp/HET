using System;

namespace Het.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RouterAttribute : Attribute
    {
        public string InputChannel { get; set; }
        public string Deadletter { get; set; }

        public RouterAttribute(string inputChannel)
        {
            this.InputChannel = inputChannel;
        }

        public RouterAttribute(string inputChannel, string deadletter)
        {
            this.InputChannel = inputChannel;
            this.Deadletter = deadletter;
        }
    }
}
