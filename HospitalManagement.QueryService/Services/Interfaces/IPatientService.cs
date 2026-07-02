using HospitalManagement.QueryService.Models.DTOs.Patient;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Result<List<PatientListDto>>> GetAllAsync();
        Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id);
        Task<Result<PatientMedicalHistoryDto>> GetMedicalHistoryAsync(int patientId);
    }
}