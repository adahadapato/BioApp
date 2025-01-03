

using System.Collections.Generic;

namespace BioApp.Models
{
    public class CafeOperatorModel
    {
        public string Operatorid { get; set; }
        public string Cafe { get; set; }
        public bool BankValidated { get; set; }
        public string Role { get; set; }
    }

    public class OperatorActivationResult
    {
        public string OperatorID { get; set; }
        public string Name { get; set; }
        public string BankValidated { get; set; }
    }

    public class SetOperatorStatusModel
    {
        public string OperatorID { get; set; }
        public string Status { get; set; }
    }

    public class SetOperatorStatusResultModel
    {
        public string OperatorID { get; set; }
        public string Status { get; set; }
        public string responseStatus { get; set; }
        public string responseComment { get; set; }
    }

    public class NewCafeOperatorModel
    {
        public string operatorid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phoneno { get; set; }
        public string state { get; set; }
    }


    public class NewCafeOperatorViewModel: NewCafeOperatorModel
    {
        public bool IsSelected { get; set; }
       
    }
    public class NewCafeOperatorApiModel
    {
        public List<NewCafeOperatorModel> CafeOperators { get; set; }
    }

    public class NewCafeOperatorResultApiModel
    {
        public string operatorid { get; set; }
        public string responseStatus { get; set; }
        public string responseComment { get; set; }
        public string valueDate { get; set; }
    }

    public class OnlineOperatorListApiModel
    {
        public string operatorId { get; set; }
        public string cafeName { get; set; }
    }
}
