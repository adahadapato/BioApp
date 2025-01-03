

using BioApp.DB;
using BioApp.Models;
using BioApp.Tools;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private bool LoginSucceed;
        private bool hasBankDetails;
        private CafeOperatorModel cafeOperatorModel;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="FileName"></param>
        public LogInPage()
        {
            InitializeComponent();
            //this.FileName = FileName;
            SafeGuiWpf.SetText(txtmsg, "");
           
        }

        public CafeOperatorModel returnCafeOperatorModel
        {
            get { return cafeOperatorModel; }
        }
        public bool IsLoginSucceeded
        {
            get { return LoginSucceed; }
        }

        public bool HasBankDetails
        {
            get { return hasBankDetails; }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        /// <summary>
        /// The Activate Button Click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                /* LongActionDialog.ShowDialog("Login",Task.Run(()=>
                 {

                 }));*/
                Operatorid = SafeGuiWpf.GetText(txtsearch).Trim();
                if (string.IsNullOrWhiteSpace(Operatorid))
                {
                    SafeGuiWpf.ShowError("Invalid Operator Id \n" +
                        "Your Id cannot be activated at this time");//, "Activate Operator", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ActivateOperator(Operatorid);
            });
        }


        private async void ActivateOperator(string id)
        {
           
            try
            {
                SafeGuiWpf.SetText(txtmsg, "Please wait... activating Operator id...");
                await Task.Delay(2000);
                //Check to see if the user ID already exist.
                var result = await ActivationStatusClass.Activate(Operatorid);
                if (result == null)
                {
                    SafeGuiWpf.ShowError("Unable to activate Cafe Operator Id at this time, Activation failed");//, "Activate", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    SafeGuiWpf.SetText(txtmsg, "");
                    LoginSucceed = false;
                    return;
                }

                hasBankDetails = true;
                cafeOperatorModel = result as CafeOperatorModel;
                if (!result.BankValidated)
                {
                    hasBankDetails = false;
                    //SafeGuiWpf.ShowWindow<OperatorBankDetailsPage, CafeOperatorModel>(FileName, result);
                    //SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
                    SafeGuiWpf.CloseWindow(this);
                    return;
                }
               
                CafeOperatorClass.WriteOperatorId(result);
                SafeGuiWpf.SetText(txtmsg, "");

                SafeGuiWpf.ShowSuccess("Your Id is activated successfully for NECO SSCE JUN/JUL 2018 Biometrics Data capture exercise\n" +
                                    "Details: Id [ " + result.Operatorid + " ] Cafe [ " + result.Cafe + " ]");//, "Activate Operator", MessageBoxButton.OK, MessageBoxImage.Information);
                                                                                                              //CreateWorkingTables();
                LoginSucceed = true;
                //SafeGuiWpf.ShowWindow<MainWindow>(this.FileName);
                SafeGuiWpf.CloseWindow(this);
                //base.Close();
                //SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
               
            }
            catch (Exception ex) { SafeGuiWpf.ShowError(ex.Message); }
            // CreateOperatorJson
        }
       
        /// <summary>
        /// Perform Operator Activation
        /// this is done in a background worker
        /// to unfreeze the UI.
        /// </summary>
        /// <param name="id"></param>
       /* private  async void WriteOperatorId(string id)
        {
            try
            {
                //The user must enter an ID
               
                SafeGuiWpf.SetText(txtmsg, "please wait... activating Operator id...");
               await Task.Delay(2000);
                //Check to see if the user ID already exist.
                var result = await ActivationStatusClass.Activate(Operatorid);
               if(result==null)
                {
                    SafeGuiWpf.ShowError("Unable to activate Cafe Operator Id at this time, Activation failed");//, "Activate", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    SafeGuiWpf.SetText(txtmsg, "");
                    return;
                }

                if (result.Operatorid !=id && result.Operatorid != "ADMIN")
                {
                    SafeGuiWpf.ShowError("Invalid Cafe Operator Id, Activation failed");//, "Activate", MessageBoxButton.OK, MessageBoxImage.Error);
                   SafeGuiWpf.SetText(txtmsg, "");
                   return;
                }
                else
                {
                    if (!result.BankValidated)
                    {
                        SafeGuiWpf.ShowWindow<OperatorBankDetailsPage, CafeOperatorModel>(FileName, result);
                        SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
                        return;
                    }
                    //A valid Operator Id was returned in the Json.
                    
                    MainWindow.Operatorid = result.Operatorid;
                    MainWindow.Cafe = result.Cafe;
                    CafeOperatorClass.WriteOperatorId(result);
                }

                SafeGuiWpf.SetText(txtmsg, "");


                if (CafeOperatorClass.IsSystemActivated())
                {
                    SafeGuiWpf.ShowSuccess("Your Id is activated successfully for NECO SSCE JUN/JUL 2018 Biometrics Data capture exercise\n" +
                                    "Details: Id [ " + result.Operatorid + " ] Cafe [ " + result.Cafe + " ]");//, "Activate Operator", MessageBoxButton.OK, MessageBoxImage.Information);
                    //CreateWorkingTables();
                   SafeGuiWpf.ShowWindow<MainWindow>(this.FileName);
                    SafeGuiWpf.SetVisible(this, Visibility.Collapsed);
                }

            }
            catch(Exception ex) {SafeGuiWpf.ShowError(ex.Message); }
        }*/

        

        /// <summary>
        /// Clean up any temp files used to run App
        /// </summary>
        private void CleanUpFiles()
        {
            //This enables the UI to stay unfrozen
            Task.Run(() =>
            {
                try
                {
                    string[] files = Directory.GetFiles(AppPathClass.FetcAppPath() + @"\", "*.bmp");
                    foreach (string f in files)
                    {
                        try
                        {
                            File.Delete(f);
                        }
                        catch { continue; }
                    }
                        

                    string[] file = Directory.GetFiles(AppPathClass.FetcAppPath() + @"\", "temp*.neco");
                    foreach (string f in file)
                    {
                        try
                        {
                            File.Delete(f);
                        }
                        catch { continue; }
                    }
                        
                }
                catch { }
                Environment.Exit(1);//Exit the App.
            });
        }

        /// <summary>
        /// The user does not intend to continue
        /// when the close button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                CleanUpFiles();
            });
            
        }

        
    }
}
