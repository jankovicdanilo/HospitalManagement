using HospitalManagement.Appointments.Models.DTOs.Treatment;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Appointments.Services.Validations
{
    public interface ITreatmentValidation
    {
        Task<Result> ValidateAll(TreatmentCreateRequestDto request);
    }
}