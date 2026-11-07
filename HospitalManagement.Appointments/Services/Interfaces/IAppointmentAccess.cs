using HospitalManagement.Shared.Common;
using System.Security.Claims;

namespace HospitalManagement.Appointments.Services.Interfaces
{
    public interface IAppointmentAccess
    {
        Result<int?> GetDoctorScope(ClaimsPrincipal user);
        Result CheckDoctor(ClaimsPrincipal user, int doctorId);
        Task<Result> CheckAppointmentAsync(ClaimsPrincipal user, int appointmentId);
        Task<Result> CheckPatientAsync(ClaimsPrincipal user, int patientId);
    }
}
