
using System.Collections.Generic;

namespace BioApp.Models
{
    public class TemplatesModel
    {
       // public string cafe { get; set; }
        public string schnum { get; set; }
        public string reg_no { get; set; }
        public byte[] finger { get; set; }
        public byte[] template { get; set; }
        public long quality { get; set; }
        
    }

    public class TemplatesUploadTempModel
    {
        // public string cafe { get; set; }
        public string schNum { get; set; }
        public string regNo { get; set; }
        public string finger { get; set; }
        public string template { get; set; }
        public long quality { get; set; }

    }

    public class TemplatesUploadModel
    {
        public string cafe { get; set; }
        public List<TemplatesUploadTempModel> templates { get; set; }
    }

    public class TemplateToVerifyModel
    {
        public string reg_no { get; set; }
        public string template1 { get; set; }
        public string quality1 { get; set; }
        public string template2 { get; set; }
        public string quality2 { get; set; }
        public string template3 { get; set; }
        public string quality3 { get; set; }
        public string template4 { get; set; }
        public string quality4 { get; set; }
    }

    public class TemplateToVerifyApiModel
    {
        public string sch_num { get; set; }
        public List<TemplateToVerifyModel> templates { get; set; }
    }
}
