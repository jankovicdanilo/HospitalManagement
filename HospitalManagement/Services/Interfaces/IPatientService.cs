using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Patient;

namespace HospitalManagement.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Result<PatientUpdateResponseDto>> UpdateAsync(PatientUpdateRequestDto request);
    }
}
