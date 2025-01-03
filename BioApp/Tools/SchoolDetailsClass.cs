using BioApp.RegistryHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Tools
{
    public class SchoolDetailsClass
    {
        private static IRegistryToken regToken= new RegistryToken();
        private static string schoolNo;
        private static string schoolName;
       

        public static string SchoolNo
        {
            get { return regToken.Getvalue("SchoolNo").ToString(); }
            set
            {
                schoolNo = value;
                regToken.Setvalue("SchoolNo", schoolNo);
            }
        }

        public static string SchoolName
        {
            get { return regToken.Getvalue("SchoolName").ToString(); }
            set
            {
                schoolName = value;
                regToken.Setvalue("SchoolName", schoolName);
            }
        }
    }
}
