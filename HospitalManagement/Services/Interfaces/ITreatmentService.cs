using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Treatment;

namespace HospitalManagement.Services.Interfaces
{
    public interface ITreatmentService
    {
        Task<Result<TreatmentCreateResponseDto>> CreateAsync(TreatmentCreateRequestDto request);
    }
}
