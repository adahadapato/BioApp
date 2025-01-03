

namespace BioApp.Models
{
    public class BankSaveModel
    {
        public string bank_code { get; set; }
        public string bank_name { get; set; }
    }

    public class UpdateBankDetailsModel
    {
        public string operatorID { get; set; }
        public string bankName { get; set; }
        public string bankAddress { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string accountSortCode { get; set; }
    }

    public class UpdateBankDetailsResult
    {
        public string operatorID { get; set; }
        public string bankName { get; set; }
        public string bankAddress { get; set; }
        public string accountName { get; set; }
        public string accountNumber { get; set; }
        public string accountSortCode { get; set; }
        public string bankValidated { get; set; }
        public string responseStatus { get; set; }
        public string responseComment { get; set; }
    }
}
