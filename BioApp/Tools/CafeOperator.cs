

namespace BioApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.IO;
    using System.Data;
    using BioApp.DB;
    using Tools;
    using Models;
    using RegistryHelper;
    using System.Threading.Tasks;

    public class CafeOperatorClass
    {
       static IRegistryToken regToken=  new RegistryToken();


        /// <summary>
        /// Writes the Cafe Operator OperatorId into our text file.
        /// </summary>
        /// <param name="id"></param> this is the Cafe OperatorId
        /// used in activating application
        public static void WriteOperatorId(CafeOperatorModel model)
        {
            try
            {
               // regToken = new RegistryToken();
                regToken.Setvalue("OperatorId", model.Operatorid);
                regToken.Setvalue("cafe", model.Cafe);
                regToken.Setvalue("ActivationStatus", "true");
                regToken.Setvalue("isLogin", "true");
                regToken.Setvalue("Role", model.Role);
            }
            catch { }
           
        }

       /* public static void LoadOperatorId()
        {
            Task.Run(() =>
            {
                try
                {
                    regToken = new RegistryToken();
                    MainWindow.Cafe = regToken.Getvalue("cafe").ToString().ToString();// model.Cafe;
                    Operatorid = regToken.Getvalue("OperatorId").ToString();// model.Operatorid;
                    MainWindow.Operatorid = Operatorid;// model.Operatorid;

                }
                catch (Exception e)
                {
                    SafeGuiWpf.MsgBox(e.Message + " " + e.StackTrace, "Get Operators", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            });
        }*/

       /* public static bool IsSystemActivated()
        {
            bool IsActivated = false;
            try
            {
                LoadOperatorId();
                IsActivated = (string.IsNullOrWhiteSpace(Operatorid)) ? false : true;
            }
            catch  {  }
            return IsActivated;
        }*/

        public  static string GetId
        {
            get{

                return regToken.Getvalue("OperatorId").ToString();
            }
        }

        public static string GetName
        {
            get { return regToken.Getvalue("Cafe").ToString(); }
        }

        public static string GetRole
        {
            get { return regToken.Getvalue("Role").ToString(); }
        }
    }
}
