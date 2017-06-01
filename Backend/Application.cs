using Het.Common;
using System;
using System.IO;

namespace Het.Backend
{
    public class Application : MarshalByRefObject
    {
        public string Name { get; private set; }

        public bool Active { get; set; } = true;

        public object[] InternalComponents { get; private set; }
        public object[] ExternalComponents { get; private set; }

        public void Init(string internalPath, string externalPath)
        {
            this.Name = new DirectoryInfo(externalPath).Name;

            this.InternalComponents = AssemblyHelper.GetInstances<ComponentAttribute>(internalPath);
            this.ExternalComponents = AssemblyHelper.GetInstances<ComponentAttribute>(externalPath);
        }

        public void FireEvents()
        {
            AssemblyHelper.Wire(this.ExternalComponents);
            AssemblyHelper.Wire(this.InternalComponents);
            AssemblyHelper.WireInternal(this.InternalComponents, this);

            AssemblyHelper.InvokePostConstruct(this.ExternalComponents);
            AssemblyHelper.InvokePostConstruct(this.InternalComponents);
        }

        public void Stop()
        {
            this.Active = false;
        }
    }
}
