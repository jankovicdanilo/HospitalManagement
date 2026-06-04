using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;

namespace HospitalManagement.Services.Validations
{
    public interface IAppointmentValidation
    {
        Task<Result> ValidateAll(AppointmentCreateRequestDto request);
        Task<Result> ValidateAll(AppointmentUpdateRequestDto request);
    }
}
