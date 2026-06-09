
using HospitalManagement.Models.DTOs.Treatment;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Services.Interfaces
{
    public interface ITreatmentService
    {
        Task<Result<TreatmentCreateResponseDto>> CreateAsync(TreatmentCreateRequestDto request);
    }
}
