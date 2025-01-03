using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Models
{
    public class DataDownloadModel
    {
        public string SchoolNo { get; set; }
        public string SchoolName { get; set; }
        public List<PersonalInfoDownloadModel> PersonalInfo { get; set; }
        public List<TemplatesDownloadModel> Templates { get; set; }
    }
  
    public class TemplatesDownloadModel
    {
        public string schnum { get; set; }
        public string reg_no { get; set; }
        public string template { get; set; }
        public int quality { get; set; }
    }
    public class PersonalInfoDownloadModel
    {
        public string schnum { get; set; }
        //public string sch_name { get; set; }
        public string reg_no { get; set; }
        public string cand_name { get; set; }
        public string sex { get; set; }
        public string passport { get; set; }
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
}
