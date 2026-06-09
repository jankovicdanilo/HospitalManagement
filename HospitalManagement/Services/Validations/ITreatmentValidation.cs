
using HospitalManagement.Models.DTOs.Treatment;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Services.Validations
{
    public interface ITreatmentValidation
    {
        Task<Result> ValidateAll(TreatmentCreateRequestDto request);
    }
}
