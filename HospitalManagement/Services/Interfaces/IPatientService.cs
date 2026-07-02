using HospitalManagement.Models.DTOs.Patient;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs;

namespace HospitalManagement.Services.Interfaces
{
    public interface IPatientService
    {

        Task<Result> Delete(int id);

        Task<Result<List<PatientListDto>>> GetAllAsync();

        Task<Result<PatientResponseDto?>> GetByIdAsync(int id);

        Task<Result<PatientCreateResponseDto?>> CreateAsync(PatientCreateRequestDto request);

        Task<Result<PatientUpdateResponseDto>> UpdateAsync(PatientUpdateRequestDto request);

        Task<Result<PatientMedicalHistoryDto>> GetMedicalHistoryAsync(int patientId);
    }
}
