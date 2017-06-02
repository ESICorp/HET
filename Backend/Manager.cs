using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Het.Backend
{
    public class Manager : MarshalByRefObject
    {
        private FileSystemWatcher FileSystemWatcher { get; set; }

        private Dictionary<string, Application> Applications { get; set; } = new Dictionary<string, Application>();

        private AppDomain[] AppDomains {get; set; }

        private object SyncRoot = new object();

        public Manager()
        {
            Task.Delay(250).ContinueWith(t => LoadApplications());

            AppDomain.CurrentDomain.DomainUnload += DomainUnload;
        }

        private void LoadApplications()
        { 
            var path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var tuple = AssemblyHelper.GetApplications(path);
            this.AppDomains  = tuple.Item1;

            this.Applications.Clear();

            foreach (var app in tuple.Item2)
            {
                this.Applications.Add(app.Name, app);
            }

            if (Directory.Exists(Path.Combine(path, "Repository")))
            {
                this.FileSystemWatcher = new FileSystemWatcher(Path.Combine(path, "Repository"), "*.*");
                this.FileSystemWatcher.Changed += OnChanged;
                this.FileSystemWatcher.Created += OnChanged;
                this.FileSystemWatcher.Renamed += OnChanged;
                this.FileSystemWatcher.Deleted += OnChanged;
                this.FileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        public void DomainUnload(object sender, EventArgs e)
        {
            lock (this.SyncRoot)
            {
                if (this.FileSystemWatcher != null)
                {
                    this.FileSystemWatcher.Dispose();
                    this.FileSystemWatcher = null;
                }

                foreach (var app in this.Applications.Values)
                {
                    app.Stop();
                }

                foreach (var appDomain in this.AppDomains) 
                {
                    AppDomain.Unload(appDomain);
                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            lock (this.SyncRoot)
            {
                if (this.FileSystemWatcher != null)
                {
                    this.FileSystemWatcher.Dispose();
                    this.FileSystemWatcher = null;

                    Trace.TraceWarning("Unload old library version");

                    foreach (var app in this.Applications.Values)
                    {
                        app.Stop();
                    }

                    foreach (var appDomain in this.AppDomains) 
                    {
                        AppDomain.Unload(appDomain);
                    }

                    Trace.TraceWarning("Reload new library version");

                    Task.Delay(250).ContinueWith(t => LoadApplications());
                }
            }
        }
    }
}
