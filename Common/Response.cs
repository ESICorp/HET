using System;
using System.Runtime.Serialization;

namespace Het.Common
{
    [Serializable]
    [DataContract]
    public class Response
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
