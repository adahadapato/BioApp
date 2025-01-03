using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Models
{
    public class FinModel
    {
        public string SchoolNo { get; set; }
        public string SchoolName { get; set; }
        public int Captured { get; set; }
        public int Balance { get; set; }
        public string Progress { get; set; }
    }

    public class UploadStatModel
    {
        public string SchoolNo { get; set; }
        public string SchoolName { get; set; }
        public int Captured { get; set; }
        public int Uploaded { get; set; }
        public int Pending { get; set; }
        public int Total { get; set; }
        public string Progress { get; set; }
        public bool IsSelected { get; set; }
    }
}
