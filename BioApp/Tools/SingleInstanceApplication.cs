

using BioApp.DB;
using BioApp.Tools;
using System;
using System.ComponentModel;
using System.IO;
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

    public class SingleInstanceApplication : System.Windows.Application
    {
        System.Collections.ObjectModel.ReadOnlyCollection<string> _commandLine;
        public SingleInstanceApplication(System.Collections.ObjectModel.ReadOnlyCollection<string> _commandLine)
        {
            this._commandLine = _commandLine;
        }

        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);
            string fileName = "";
            if (_commandLine.Count > 1)
                fileName = _commandLine[1].ToString();
            else
                fileName = "";
            bool IsActive = Convert.ToBoolean(RegistryValues.GetRegistryValue("ActivationStatus"));
            string Operatorid = RegistryValues.GetRegistryValue("OperatorId").ToString();
            string cafe = RegistryValues.GetRegistryValue("cafe").ToString();
            if (!IsActive)
            {
                CreateDB();
                CreateSubjectTables();
                CreateStaffTables();
            }
            if(string.IsNullOrWhiteSpace(Operatorid) || string.IsNullOrWhiteSpace(cafe))
            {
                LogInPage window = new LogInPage(fileName);
                window.Show();
            }
            else
            {
                MainWindow window = new MainWindow(fileName);
                CafeOperatorClass.LoadOperatorId();
                window.Show();
            }
           /* if (CafeOperatorClass.IsSystemActivated())
            {
                // Create our MainWindow and show it
               
            }
            else
            {
                
            }*/

        }

        public void Activate()
        {
            // Reactivate the main window
            MainWindow.Activate();
        }


        private void CreateDB()
        {
            IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
            dl.CreateDataBase();
        }

        /// <summary>
        /// Creates the DB and default working
        /// files as subject ref, et all
        /// this is run only at first activation
        /// </summary>
        private void CreateSubjectTables()
        {
            try
            {
                string SubjFile = AppPathClass.FetcAppPath() + @"\Subject.neco";
                string temp = AppPathClass.FetcAppPath() + @"\temp.neco";
                if (File.Exists(temp))
                    File.Delete(temp);

                CryptograhyClass.Decrypt(SubjFile, temp, CryptograhyClass.EncryptionPWD);
                JsonLayer.CreateSubjectsStorageJson(temp);
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Create Subject files", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void CreateStaffTables()
        {
            try
            {
                IGRDal dl = SQLDalFactory.GetDal(GrConnector.AccessSQLDal);
                string StaffFile = AppPathClass.FetcAppPath() + @"\Staff.neco";
                string temp = AppPathClass.FetcAppPath() + @"\temp.neco";
                if (File.Exists(temp))
                    File.Delete(temp);

                CryptograhyClass.Decrypt(StaffFile, temp, CryptograhyClass.EncryptionPWD);
                JsonLayer.CreateStaffStorageJson(temp);
             //   ShowStatusText("Done ...");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Create Staff files", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

    }
    
}
