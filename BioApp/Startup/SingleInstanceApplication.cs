

using BioApp.Activities;
using BioApp.Models;
using BioApp.RegistryHelper;
using BioApp.Tools;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace BioApp
{
    public class EntryPoint
    {
        [STAThread]
        public static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            SingleInstanceController controller = new SingleInstanceController();
            controller.Run(args);
        }
    }

    public class SingleInstanceApplication : Application
    {
        //public readonly ToastViewModel _vm = new ToastViewModel();
        IRegistryToken regToken = new RegistryToken();
        private bool LoginSucceed , OpthasBankDetails ;
        private CafeOperatorModel cafeOperatorModel;
        System.Collections.ObjectModel.ReadOnlyCollection<string> _commandLine;
        public SingleInstanceApplication(System.Collections.ObjectModel.ReadOnlyCollection<string> _commandLine)
        {
            this._commandLine = _commandLine;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            string fileName = "";
            if (_commandLine.Count > 1)
                fileName = _commandLine[1].ToString();
            else
                fileName = "";
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            MainWindow window = new MainWindow(fileName);
           

           
            bool IsActive = Convert.ToBoolean(regToken.Getvalue("ActivationStatus"));

            if (!IsActive)
            {
                var result = await CreateDB();
                if (!result)
                {
                    SafeGuiWpf.ShowError("The process is unable to continue\n" +
                                    "Please contact NECO Support");//, "Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                

                if (PerformLoginAction())
                    window.Show();
            }
            else
            {
                bool IsLogin = Convert.ToBoolean(regToken.Getvalue("IsLogin"));
                if (IsLogin)
                {
                    window.Show();
                }
                else
                {
                    if (PerformLoginAction())
                        window.Show();
                }
            }
            
        }

        private bool PerformLoginAction()
        {
            LogInPage loginPage = new LogInPage();
            OperatorBankDetailsPage operatorBankDetailsPage = new OperatorBankDetailsPage();

            loginPage.ShowDialog();

            LoginSucceed = loginPage.IsLoginSucceeded;
            OpthasBankDetails = loginPage.HasBankDetails;
            cafeOperatorModel = loginPage.returnCafeOperatorModel;


            if (!LoginSucceed)
            {
                Environment.Exit(0);
            }
            if (!OpthasBankDetails)
            {
                operatorBankDetailsPage.ShowDialog();
                operatorBankDetailsPage.cafeOperatorModel = cafeOperatorModel;
                OpthasBankDetails = operatorBankDetailsPage.IsOperationSucceeded;
            }

            if (OpthasBankDetails && LoginSucceed)
                return true;
            else
                return false;
        }
        public void Activate()
        {
            // Reactivate the main window
            MainWindow.Activate();
        }


        private async Task<bool> CreateDB()
        {
            bool IsSuccess = false;
            using (WriteDataClass wd = new WriteDataClass())
            {
                 IsSuccess = await wd.CreateDataBase();

                if (IsSuccess)
                    IsSuccess = await CreateStaffTables();
                else
                    return false;

                if (IsSuccess)
                    IsSuccess = await CreateSubjectTables();
                else
                    return false;

                if (IsSuccess)
                    IsSuccess = await CreateBankTables();
                else
                    return false;

                if (IsSuccess)
                    IsSuccess = await CreateSateTables();
                else
                    return false;

                return IsSuccess;
            }
        }

        /// <summary>
        /// Creates the DB and default working
        /// files as subject ref, et all
        /// this is run only at first activation
        /// </summary>
        private async Task<bool> CreateSubjectTables()
        {
            return await Task.FromResult(LongActionDialog.ShowDialog<bool>("Create Subjects ... ", Task<bool>.Run(() =>
            {
                try
                {
                    string SubjFile = AppPathClass.FetcAppPath() + @"\Subject.neco";
                    string temp = AppPathClass.FetcAppPath() + @"\temp.neco";
                    if (File.Exists(temp))
                        File.Delete(temp);

                    CryptograhyClass.Decrypt(SubjFile, temp, CryptograhyClass.EncryptionPWD);
                    JsonLayer.CreateSubjectsStorageJson(temp);
                    return true;

                }
                catch (Exception e)
                {
                   SafeGuiWpf.ShowError("Create Subject files\n" + e.Message);//, "Create Subject files", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            })));
           
        }

        /// <summary>
        /// Creates the DB and default working
        /// files as subject ref, et all
        /// this is run only at first activation
        /// </summary>
        private async Task<bool> CreateBankTables()
        {
          return await Task.FromResult( LongActionDialog.ShowDialog<bool>("Creating Banks ... ", Task<bool>.Run( () =>
            {
                try
                {
                    string BankFile = AppPathClass.FetcAppPath() + @"\bank.neco";
                    string temp = AppPathClass.FetcAppPath() + @"\temp.neco";
                    if (File.Exists(temp))
                        File.Delete(temp);

                    CryptograhyClass.Decrypt(BankFile, temp, CryptograhyClass.EncryptionPWD);
                    JsonLayer.CreateBankStorageJson(temp);

                    return true;

                }
                catch (Exception e)
                {
                  SafeGuiWpf.ShowError("Create Bank files\n" +
                                 e.Message);//, "Create Bank files", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            })));
            //return await Task.FromResult(result);
            
        }

        /// <summary>
        /// Creates the DB and default working
        /// files as subject ref, et all
        /// this is run only at first activation
        /// </summary>
        private async Task<bool> CreateSateTables()
        {
            return await Task.FromResult(LongActionDialog.ShowDialog<bool>("Create States ...", Task<bool>.Run(() =>
            {
                try
                {
                    string SubjFile = AppPathClass.FetcAppPath() + @"\state.neco";
                    string temp = AppPathClass.FetcAppPath() + @"\temp.neco";
                    if (File.Exists(temp))
                        File.Delete(temp);

                    CryptograhyClass.Decrypt(SubjFile, temp, CryptograhyClass.EncryptionPWD);
                    JsonLayer.CreateStateStorageJson(temp);
                    return true;

                }
                catch (Exception e)
                {
                   SafeGuiWpf.ShowError("Create State files\n" +
                                 e.Message);//, "Create State files", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            })));

        }

        /// <summary>
        /// Create Staff Data Table from Embeded Staff.neco File
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CreateStaffTables()
        {
            return await Task.FromResult(LongActionDialog.ShowDialog<bool>("Create Staff file ...", Task.Run(() =>
            {
                try
                {
                    string StaffFile = AppPathClass.FetcAppPath() + @"\Staff.neco";
                    string temp = AppPathClass.FetcAppPath() + @"\temp.neco";
                    if (File.Exists(temp))
                        File.Delete(temp);

                    CryptograhyClass.Decrypt(StaffFile, temp, CryptograhyClass.EncryptionPWD);
                    JsonLayer.CreateStaffStorageJson(temp);
                    return true;
                }
                catch (Exception e)
                {
                  SafeGuiWpf.ShowError("Create Staff files\n" +
                                 e.Message);//, "Create Staff files", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return false;
            })));
           
        }

        /// <summary>
        /// Loads a resource dictionary from a URI and merges it into the application resources.
        /// </summary>
        /// <param name="resourceLocater">URI of resource dictionary</param>
        public static void MergeResourceDictionary(Uri resourceLocater)
        {
            if (Application.Current != null)
            {
                var dictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
                Application.Current.Resources.MergedDictionaries.Add(dictionary);
            }
        }

    }
    
}
