using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Treatment;

namespace HospitalManagement.Services.Validations
{
    public interface ITreatmentValidation
    {
        Task<Result> ValidateAll(TreatmentCreateRequestDto request);
    }
}
