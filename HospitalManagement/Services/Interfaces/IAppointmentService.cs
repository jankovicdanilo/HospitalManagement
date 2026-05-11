using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request);
    }
}
