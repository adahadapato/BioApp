using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Models
{
    public class TotalSummryApiModel
    {
        public string operatorId { get; set; }
        public string cafe { get; set; }
        public long total { get; set; }
    }

    public class SchoolSummrySaveModel
    {
        public string operatorId { get; set; }
        public string schoolNo { get; set; }
        public string schoolName { get; set; }
        public long total { get; set; }
    }

}
