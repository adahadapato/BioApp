using Microsoft.Win32;
using System;


namespace BioApp.RegistryHelper
{

    public interface IRegistryToken
    {
        string Getvalue(string regKey);
        void Setvalue(string regKey, string rValue);
    }
    public class RegistryToken : IRegistryToken
    {
        private const string RegEntryKey = @"software\National Examinations Council\Biometrics\";
        public string Getvalue(string regKey)
        {
            try
            {
                RegistryKey mICParams = Registry.CurrentUser;
                mICParams = mICParams.OpenSubKey(RegEntryKey, false);
                return mICParams.GetValue(regKey).ToString();

            }
            catch (Exception ex)
            {
                // Message = "Error " + ex.Message;
            }
            return string.Empty;
        }

        public void Setvalue(string regKey, string rValue)
        {
            try
            {
                RegistryKey mICParams = Registry.CurrentUser;
                mICParams = mICParams.OpenSubKey(RegEntryKey, true);
                mICParams.SetValue(regKey, rValue);
                mICParams.Close();
                //return true;
            }
            catch //(Exception ex)
            {
                // Message="Error: "+ex.Message;
            }
            //return false;
        }
    }
}
