using HospitalManagement.Appointments.Models.Domain;
using HospitalManagement.Appointments.Services.Calculators.Results;

namespace HospitalManagement.Appointments.Services.Calculators.Interfaces
{
    public interface IAppointmentDiscountCalculator
    {
        DiscountResult Calculate(ICollection<AppointmentProcedure> procedures);
    }
}