using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Het.Backend
{
    public class Manager : MarshalByRefObject
    {
        private FileSystemWatcher FileSystemWatcher { get; set; }

        private Dictionary<string, Application> Applications { get; set; } = new Dictionary<string, Application>();

        private object SyncRoot = new object();

        public Manager()
        {
            this.Init();

            AppDomain.CurrentDomain.DomainUnload += DomainUnload;
        }

        private void Init()
        { 
#if DEBUG
            var path = @"C:\Users\Andres\Source\Workspaces\Het\";
#else
            var path = AppDomain.CurrentDomain.BaseDirectory;
#endif
            var applications = AssemblyHelper.GetApplications(path);

            this.Applications.Clear();

            foreach (var app in applications)
            {
                this.Applications.Add(app.Name, app);
            }

            foreach (var app in applications)
            {
                app.FireEvents();
            }

            if (Directory.Exists(Path.Combine(path, "repository")))
            {
                this.FileSystemWatcher = new FileSystemWatcher(Path.Combine(path, "repository"), "*.*");
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

                    Trace.TraceWarning("Reload new library version");

                    this.Init();
                }
            }
        }
    }
}
