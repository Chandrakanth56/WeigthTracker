using System.Security.Principal;

namespace WeightTrackerUI.Components.RequestModel
{
    public class Farmer
    {
        public int FarmerId { get; set; }       // matches "farmerId"
        public int? VendorId { get; set; }      // matches "vendorId"
        public string FarmerName { get; set; }  // matches "farmerName"
        public string FarmerEmail { get; set; } // matches "farmerEmail"
        public string PasswordHash { get; set; } // matches "passwordHash"
        public object Vendor { get; set; }      // matches "vendor"
        public List<object> Weights { get; set; } // matches "weights"
    }
}
