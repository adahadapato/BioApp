using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BioApp.Models
{
    public class PersonalInfoModel
    {
        public string schnum { get; set; }
        //public string sch_name { get; set; }
        public string reg_no { get; set; }
        public string cand_name { get; set; }
        public string sex { get; set; }//HasPassport
        public bool HasPassport { get; set; }
        public byte[] passport { get; set; }
        public string subj1 { get; set; }
        public string subj2 { get; set; }
        public string subj3 { get; set; }
        public string subj4 { get; set; }
        public string subj5 { get; set; }
        public string subj6 { get; set; }
        public string subj7 { get; set; }
        public string subj8 { get; set; }
        public string subj9 { get; set; }    
    }

    public class PersonalInfoViewModel
    {
        public string schnum { get; set; }
        //public string sch_name { get; set; }
        public string reg_no { get; set; }
        public string cand_name { get; set; }
        public string sex { get; set; }
        public byte[] passport { get; set; }
        public BitmapImage Pict { get; set; }
        public string subj1 { get; set; }
        public string subj2 { get; set; }
        public string subj3 { get; set; }
        public string subj4 { get; set; }
        public string subj5 { get; set; }
        public string subj6 { get; set; }
        public string subj7 { get; set; }
        public string subj8 { get; set; }
        public string subj9 { get; set; }
        public string fingers { get; set; }
        public string Progress { get; set; }
    }
}
