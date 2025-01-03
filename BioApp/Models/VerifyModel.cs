using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioApp.Models
{
    /// <summary>
    /// Model for Candidate Verification
    /// </summary>
    public class VerifyModel
    {
        public string schnum { get; set; }
        public string reg_no { get; set; }
        public string code { get; set; }
        public string subj_code { get; set; }
        public string subject { get; set; }
        public string paper { get; set; }
        public string Verified { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
        public string verifyDate { get; set; }
        public string cand_name { get; set; }
    }

    public class VerifyViewModel:VerifyModel
    {
      public int S_No { get; set; }
    }


    public class CandidateModel
    {
        public string Reg_No { get; set; }
        public string Cand_Name { get; set; }
        public string Subject_Code { get; set; }
        public string Subject_Name { get; set; }
        public string Paper_Code { get; set; }
        public string Paper_Name { get; set; }
        public int Status { get; set; }
        public string remark { get; set; }
    }

    public class VerifyCandidateApiModel
    {
        public string AccessKey { get; set; }
        public string SecurityKey { get; set; }
        public string SchoolNo { get; set; }
        public string SchoolName { get; set; }
        public string ExamYear { get; set; }
        public string ExamType { get; set; }
        public int ResponseStatus { get; set; }
        public string ResponseMessage { get; set; }
        public List<CandidateModel> Candidates { get; set; }
    }
}
