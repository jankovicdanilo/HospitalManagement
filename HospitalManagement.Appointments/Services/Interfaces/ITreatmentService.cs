using HospitalManagement.Appointments.Models.DTOs.Treatment;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Appointments.Services.Interfaces
{
    public interface ITreatmentService
    {
        Task<Result<TreatmentCreateResponseDto>> CreateAsync(TreatmentCreateRequestDto request);
    }
}