using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Result<PagedResult<PatientListDto>>> GetAllAsync(PatientFilterDto filter);
        Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id);
        Task<Result<PatientMedicalHistoryDto>> GetMedicalHistoryAsync(int patientId);
    }
}