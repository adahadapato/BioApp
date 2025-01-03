

namespace BioApp
{
    using System.IO;
    public class AppPathClass
    {
        public static string FetcAppPath()
        {
            string APP_PATH = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            return APP_PATH;
        }
    }
}
