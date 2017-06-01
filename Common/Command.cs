using System;
using System.Runtime.Serialization;

namespace Het.Common
{
    [Serializable]
    [DataContract]
    public class Command 
    {
        [DataMember]
        public Request Request { get; set; }

        [DataMember]
        public Response Response { get; set; }

        [NonSerialized]
        private Context context;
        public Context Context { get { return this.context; } set { this.context = value; } }

        [DataMember]
        public Response[] Partial { get; set; }

        [DataMember]
        public int Ordinal { get; set; }

        [DataMember]
        public int Length { get; set; }
   } 
}