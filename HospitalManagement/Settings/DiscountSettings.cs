namespace HospitalManagement.Settings
{
    public class DiscountSettings
    {
        public int Tier1Percentage { get; set; }
        public int Tier1MinCount { get; set; }
        public int Tier2Percentage { get; set; }
        public int Tier2MinCount { get; set; }
        public int Tier3Percentage { get; set; }
        public int Tier3MinCount { get; set; }
        public int MaxDiscount { get; set; }
    }
}
