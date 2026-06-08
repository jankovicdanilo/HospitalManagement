using HospitalManagement.Models.Domain;
using HospitalManagement.Services.Calculators.Interfaces;
using HospitalManagement.Services.Calculators.Results;

namespace HospitalManagement.Services.Calculators.Implementations
{
    public class AppointmentDiscountCalculator : IAppointmentDiscountCalculator
    {
        public DiscountResult Calculate(ICollection<AppointmentProcedure> procedures)
        {
            
            var totalCost = procedures.Sum(ap => ap.Procedure.Price);
            var procedureCount = procedures.Count;
            decimal discount = 0;

            if(procedureCount >= 3)
            {
                discount = totalCost * 0.03m;
            }
            if(procedureCount >= 5)
            {
                discount = totalCost * 0.04m;
            }
            if(procedureCount >= 7)
            {
                discount = totalCost * 0.05m;
            }
            if(discount > 100)
            {
                discount = 100;
            }

            totalCost = totalCost - discount;
            var result = new DiscountResult(totalCost, discount);

            return result;
        }
    }
}
