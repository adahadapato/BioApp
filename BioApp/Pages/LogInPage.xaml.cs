

using BioApp.DB;
using BioApp.Tools;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace BioApp
{
    /// <summary>
    /// Interaction logic for LogInPage.xaml
    /// The login/Activation Page.
    /// </summary>
    public partial class LogInPage : Window
    {
        private string FileName;
        private string Operatorid = "";
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName"></param>
        public LogInPage(string FileName)
        {
            InitializeComponent();
            this.FileName = FileName;
            ShowStatusText("");
           
        }


        /// <summary>
        /// The Activate Button Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            
            Operatorid = this.txtsearch.Text.Trim();
            WriteOperatorId(Operatorid);
            //CreateWorkingTables();
        }

       
        /// <summary>
        /// Perform Operator Activation
        /// this is done in a background worker
        /// to unfreeze the UI.
        /// </summary>
        /// <param name="id"></param>
        private void WriteOperatorId(string id)
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    //The user must enter an ID
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        MessageBox.Show("Invalid Operator Id \n Your Id cannot be activated at this time", "Activate Operator", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ShowStatusText("please wait... activating Operator id...");
                    Thread.Sleep(3000);
                     //Check to see if the user ID already exist.
                    ActivationStatusClass.Activate(Operatorid);
                    ShowStatusText("");
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    if (CafeOperatorClass.IsSystemActivated())
                    {
                        MessageBox.Show("Your Id is activated successfully for NECO SSCE NOV/DEC 2017 Biometrics Data capture exercise\n" +
                                        "Details: Id [ " + id + " ] Cafe [ " + MainWindow.Cafe + " ]", "Activate Operator", MessageBoxButton.OK, MessageBoxImage.Information);
                        //CreateWorkingTables();
                        MainWindow w = new MainWindow(FileName);
                        w.Show();
                        this.Close();
                    }
                };
                worker.RunWorkerAsync();
            }
            catch { }
        }

        

        /// <summary>
        /// Clean up any temp files used to run App
        /// </summary>
        private void CleanUpFiles()
        {
            BackgroundWorker worker = new BackgroundWorker();//This enables the UI to stay unfrozen
            worker.DoWork += (o, ea) =>
            {
                try
                {
                    string[] files = Directory.GetFiles(AppPathClass.FetcAppPath() + @"\", "*.bmp");
                    foreach (string f in files)
                        File.Delete(f);

                    string[] file = Directory.GetFiles(AppPathClass.FetcAppPath() + @"\", "temp*.neco");
                    foreach (string f in file)
                        File.Delete(f);
                }
                catch { }

            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                Environment.Exit(1);//Exit the App.
            };

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// The user does not intend to continue
        /// when the close button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CleanUpFiles();
        }

        /// <summary>
        /// delegate to display message to the UI 
        /// while performing background action
        /// </summary>
        /// <param name="Text"></param>
        private void ShowStatusText(string Text)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                txtmsg.Text = Text;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      txtmsg.Text = Text;
                  }));
            }

        }
    }
}
