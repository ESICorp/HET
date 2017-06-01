namespace Het.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.HetInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // ProcessInstaller
            // 
            this.ProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.ProcessInstaller.Password = null;
            this.ProcessInstaller.Username = null;
            // 
            // HetInstaller
            // 
            this.HetInstaller.Description = "Entreprise Application Integration";
            this.HetInstaller.DisplayName = "Het Service";
            this.HetInstaller.ServiceName = "Het";
            this.HetInstaller.ServicesDependedOn = new string[] {
        "MSMQ"};
            this.HetInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.ProcessInstaller,
            this.HetInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller ProcessInstaller;
        private System.ServiceProcess.ServiceInstaller HetInstaller;
    }
}