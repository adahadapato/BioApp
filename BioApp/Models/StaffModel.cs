

using System.Collections.Generic;

namespace BioApp.Models
{
    public class StaffModel
    {
        public string per_no { get; set; }
        public string name { get; set; }
    }

    public class StaffRootObject
    {
        public List<StaffModel> staff { get; set; }
    }
}
