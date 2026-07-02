using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.CommandService.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Result<PatientCreateResponseDto?>> CreateAsync(PatientCreateRequestDto request);
        Task<Result<PatientUpdateResponseDto>> UpdateAsync(PatientUpdateRequestDto request);
        Task<Result> Delete(int id);
    }
}