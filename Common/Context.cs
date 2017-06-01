using System;
using System.Reflection;

namespace Het.Common
{
    public class Context
    {
        public string Id { get; set; }
        public object Component { get; set; }

        public MethodInfo MethodInfo { get; set; }

        public Attribute Attribute { get; set; }
    }
}
