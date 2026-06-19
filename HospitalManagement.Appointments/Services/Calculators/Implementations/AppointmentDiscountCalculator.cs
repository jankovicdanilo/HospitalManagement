using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Services.Calculators.Interfaces;
using HospitalManagement.Appointments.Services.Calculators.Results;
using HospitalManagement.Appointments.Settings;
using Microsoft.Extensions.Options;

namespace HospitalManagement.Appointments.Services.Calculators.Implementations
{
    public class AppointmentDiscountCalculator : IAppointmentDiscountCalculator
    {
        private readonly DiscountSettings discountSettings;

        public AppointmentDiscountCalculator(IOptions<DiscountSettings> discountSettings)
        {
            this.discountSettings = discountSettings.Value;
        }

        public DiscountResult Calculate(ICollection<AppointmentProcedure> procedures)
        {
            var totalCost = procedures.Sum(ap => ap.ProcedurePrice);
            var procedureCount = procedures.Count;
            decimal discount = 0;

            if (procedureCount >= discountSettings.Tier1MinCount)
                discount = totalCost * discountSettings.Tier1Percentage / 100m;

            if (procedureCount >= discountSettings.Tier2MinCount)
                discount = totalCost * discountSettings.Tier2Percentage / 100m;

            if (procedureCount >= discountSettings.Tier3MinCount)
                discount = totalCost * discountSettings.Tier3Percentage / 100m;

            if (discount > discountSettings.MaxDiscount)
                discount = discountSettings.MaxDiscount;

            totalCost = totalCost - discount;

            return new DiscountResult(totalCost, discount);
        }
    }
}