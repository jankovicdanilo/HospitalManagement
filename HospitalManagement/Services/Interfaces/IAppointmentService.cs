using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;

namespace HospitalManagement.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<Result<CreateAppointmentResponseDto>> CreateAsync(CreateAppointmentRequestDto request);
    }
}
