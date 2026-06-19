using HospitalManagement.Shared.Common;
using HospitalManagement.Appointments.Models.DTOs.Appointment;

namespace HospitalManagement.Appointments.Services.Validations
{
    public interface IAppointmentValidation
    {
        Task<Result> ValidateAll(AppointmentCreateRequestDto request);
        Task<Result> ValidateAll(AppointmentUpdateRequestDto request);
    }
}