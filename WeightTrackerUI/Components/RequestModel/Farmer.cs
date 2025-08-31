using System.Security.Principal;

namespace WeightTrackerUI.Components.RequestModel
{
    public class Farmer
    {
        public int FarmerId { get; set; }       
        public int? VendorId { get; set; }      
        public string FarmerName { get; set; }  
        public string FarmerEmail { get; set; } 
        public string PasswordHash { get; set; } 
        public object Vendor { get; set; }      
        public List<Weight> Weights { get; set; } 
    }
}
