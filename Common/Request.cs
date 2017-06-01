using System;
using System.Runtime.Serialization;

namespace Het.Common
{
    [Serializable]
    [DataContract]
    public class Request
    {
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public string InputChannel { get; set; }

        [DataMember]
        public string OutputChannel { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public long TimeoutSeconds { get; set; } = 60;
    }
}
