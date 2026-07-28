using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Result<PagedResult<PatientListDto>>> GetAllAsync(int pageNumber, int pageSize);
        Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id);
        Task<Result<PatientMedicalHistoryDto>> GetMedicalHistoryAsync(int patientId);
    }
}