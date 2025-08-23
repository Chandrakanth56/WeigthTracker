namespace WeightTrackerUI.Components.RequestModel
{
    public class Weight
    {
        public int weightId { get; set; }
        public int? farmerId { get; set; }
        public double weights { get; set; }
        public DateTime? timestamp { get; set; }
        public string farmer { get; set; }
        
    }
}
