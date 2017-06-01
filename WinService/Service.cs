using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using Het.Backend;
using System.IO;

namespace Het.Service
{
    public partial class Het : ServiceBase
    {
        private ServiceHost Host { get; set; }
        private AppDomain CurrentAppDomain { get; set; }

        public static void Main(string[] args)
        {
            var het = new Het();

            if (Environment.UserInteractive)
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
            var setup = new AppDomainSetup();
            setup.ApplicationName = "Backend";
            setup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //setup.PrivateBinPath = directories[i];

            var configFile = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Het.Backend.dll.config");
            if (File.Exists(configFile))
            {
                setup.ConfigurationFile = configFile;
            }

            this.CurrentAppDomain = AppDomain.CreateDomain("Backend", AppDomain.CurrentDomain.Evidence, setup);
            this.CurrentAppDomain.CreateInstance(typeof(Manager).Assembly.FullName, typeof(Manager).FullName);

            this.Host = new ServiceHost(typeof(Dispatcher), new Uri("http://0.0.0.0:9000/"));
            
            var endpoint = this.Host.AddServiceEndpoint(
                typeof(Dispatcher), new WebHttpBinding(WebHttpSecurityMode.None), "Het");

            endpoint.Behaviors.Add(new WebHttpBehavior());

            this.Host.Open();
        }


        protected override void OnStop()
        {
            if (this.Host != null)
            {
                this.Host.Close();
            }

            if (this.CurrentAppDomain != null)
            {
                AppDomain.Unload(this.CurrentAppDomain);
            }
        }
    }
}
