namespace HospitalManagement.Services.Calculators.Results
{
    public class DiscountResult
    {
        public decimal TotalCost { get; set; }
        public decimal Discount { get; set; }

        public DiscountResult(decimal totalCost, decimal discount)
        {
            TotalCost = totalCost;
            Discount = discount;
        }
    }
}
