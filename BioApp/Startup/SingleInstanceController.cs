

namespace BioApp
{
    using System;
    using Microsoft.VisualBasic.ApplicationServices;
    using System.Windows;

    public class SingleInstanceController :  WindowsFormsApplicationBase
    {
    
        
        private SingleInstanceApplication _application;
        private System.Collections.ObjectModel.ReadOnlyCollection<string> _commandLine;
        public SingleInstanceController()
        {
            //this.file = file;
            // Set whether the application is single instance
            this.IsSingleInstance = true;

            this.StartupNextInstance += new
              StartupNextInstanceEventHandler(this_StartupNextInstance);
        }

             
    

        void this_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            // Here you get the control when any other instance is
            // invoked apart from the first one.
            // You have args here in e.CommandLine.

            // You custom code which should be run on other instances
           Tools.SafeGuiWpf.MsgBoxEx("Application instance", "Application is already running",  
               MessageBoxButton.OK, WpfMessageBox.MessageBoxImage.Error);
        }

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs eventArgs)
        {
            // First time _application is launched
            _commandLine = eventArgs.CommandLine;
            _application = new SingleInstanceApplication(_commandLine);
            _application.Run();
            return false;
        }
       /* protected override void OnCreateMainForm()
        {
            var _application = new App();
            _application.Run();
            // Instantiate your main application form
            //this.MainForm = new MainWindow();
            //this.MainWindow = new MainWindow();

        }*/


    }
}
