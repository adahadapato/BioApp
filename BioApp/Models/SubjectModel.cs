using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Models
{
    public class SubjectModel
    {
        public string Code { get; set; }
        public string subj_code { get; set; }
        public string Subject { get; set; }
        public string Paper { get; set; }
        public string Descript { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }
    }

    public class SubjectViewModel
    {
        public string Code { get; set; }
        public string subj_code { get; set; }
        public string Subject { get; set; }
        public string Paper { get; set; }
        public string Descript { get; set; }
    }


    public class SubjectsModel
    {
        public List<SubjectModel> Subjects { get; set; }

    }
}
