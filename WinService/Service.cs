using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using Het.Backend;
using System.IO;
using System.Runtime.Remoting;

namespace Het.Service
{
    public partial class Het : ServiceBase
    {
        private ServiceHost Host { get; set; }
        private AppDomain BackendAppDomain { get; set; }

        private ObjectHandle Backend {get; set; }

        public static void Main(string[] args)
        {
            var het = new Het();

            if (Environment.UserInteractive || (
                args.Length > 0 && args[0] == "Debug"))
            {
                het.OnStart(args);

                Console.WriteLine("Press any key to stop");
                Console.ReadKey();

                het.OnStop();
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { het });
            }
        }


        public Het()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var manager = typeof(Manager);
            var dispatcher = typeof(Dispatcher);

            var setup = new AppDomainSetup();
            setup.ApplicationName = "Backend";
            setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            setup.LoaderOptimization = LoaderOptimization.MultiDomainHost;

            var configFile = Path.Combine(setup.ApplicationBase, "Het.Backend.dll.config");
            if (File.Exists(configFile))
            {
                setup.ConfigurationFile = configFile;
            }

            this.BackendAppDomain = AppDomain.CreateDomain("Backend", null, setup);
            this.Backend = this.BackendAppDomain.CreateInstance(manager.Assembly.FullName, manager.FullName);

            this.Host = new ServiceHost(dispatcher, new Uri("http://0.0.0.0:9000/"));

            var endpoint = this.Host.AddServiceEndpoint(
                dispatcher, new WebHttpBinding(WebHttpSecurityMode.None), "Het");

            endpoint.Behaviors.Add(new WebHttpBehavior());

            this.Host.Open();
        }


        protected override void OnStop()
        {
            try 
            {
                if (this.Host != null)
                {
                    this.Host.Close();
                }
            }
            catch { }

            try 
            {
                if (this.BackendAppDomain != null)
                {
                    AppDomain.Unload(this.BackendAppDomain);
                }
            }
            catch { }
        }
    }
}
