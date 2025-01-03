
using System.Collections.Generic;



namespace BioApp.Models
{
    public class TemplatesUploadResultModel
    {
        public string regNo { get; set; }
        public string status { get; set; }
        public string schNum { get; set; }
    }

    public class CandidateConfirmationModel
    {
        public string RegNo { get; set; }
        public string Status { get; set; }
        public string ValueDate { get; set; }
        public string schNum { get; set; }
    }

    public class TemplateUploadConfirmationToJideModel
    {
        public string cafeid { get; set; }
        public List<CandidateConfirmationModel> Candidates { get; set; }
    }

    /* public class TemplatesUploadResultModel
     {
         public List<Templates> Templates { get; set; }
     }*/

    public class SummaryUploadResultModel
    {
        public string responseStatus { get; set; }
        public string responseMessage { get; set; }
    }

}
