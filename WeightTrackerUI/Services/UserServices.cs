using WeightTrackerUI.Components.RequestModel;

namespace WeightTrackerUI.Services
{
    public class UserServices
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public int FarmerId { get; set; }
        public string FarmerName { get; set; } = string.Empty;

        public Farmer farmer { get; set; }

        public int TotalWeight { get; set; }
    }
}
