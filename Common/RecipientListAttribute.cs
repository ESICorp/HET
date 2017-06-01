using System;

namespace Het.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RecipientListAttribute : Attribute
    {
        public string InputChannel { get; set; }
        public string[] OutputChannelList { get; set; }
        public string Deadletter { get; set; }

        public RecipientListAttribute(string inputChannel)
        {
            this.InputChannel = inputChannel;
        }

        public RecipientListAttribute(string inputChannel, string[] ouputChannelList)
        {
            this.InputChannel = inputChannel;
            this.OutputChannelList = ouputChannelList;
        }

        public RecipientListAttribute(string inputChannel, string deadletter)
        {
            this.InputChannel = inputChannel;
            this.Deadletter = deadletter;
        }
        public RecipientListAttribute(string inputChannel, string[] outputChannelList, string deadletter)
        {
            this.InputChannel = inputChannel;
            this.OutputChannelList = outputChannelList;
            this.Deadletter = deadletter;
        }
    }
}
