using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Patient;

namespace HospitalManagement.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Result<List<PatientListResponseDto>>> GetAllAsync();

        Task<Result<PatientResponseDto?>> GetByIdAsync(int id);
    }
}
