using HospitalManagement.Models.Domain;
using HospitalManagement.Services.Calculators.Results;

namespace HospitalManagement.Services.Calculators.Interfaces
{
    public interface IAppointmentDiscountCalculator
    {
        DiscountResult Calculate(ICollection<AppointmentProcedure> procedures);
    }
}
