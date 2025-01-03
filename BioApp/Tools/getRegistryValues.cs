using System;
using Microsoft.Win32;
using System.Windows.Forms;

namespace BioApp
{
    static class RegistryValues
    {
        static string rValue;
        public static void UpdateRegistry(string regKey,string rValue)
        {
            try
            {
                RegistryKey mICParams = Registry.CurrentUser;
                mICParams = mICParams.OpenSubKey(@"software\National_Examinations_Council\Biometrics", true);
                mICParams.SetValue(regKey,rValue);
                mICParams.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Registry values");
            }

        }

        public static string GetRegistryValue(string regKey)
        {
            try
            {
                RegistryKey mICParams = Registry.CurrentUser;
                mICParams = mICParams.OpenSubKey(@"software\National_Examinations_Council\Biometrics", false);
                rValue = mICParams.GetValue(regKey).ToString();
                
            }
            catch (Exception ex)
            {
                rValue = "Error: "+ex.Message;
            }
            return rValue;
        }
    }
}
