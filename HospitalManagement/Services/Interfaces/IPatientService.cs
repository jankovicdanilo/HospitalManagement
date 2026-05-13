using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Patient;

namespace HospitalManagement.Services.Interfaces
{
    public interface IPatientService
    {
        Task<Result<List<PatientListDto>>> GetAllAsync();
        Task<Result<PatientGetByIdDto?>> GetByIdAsync(int id);
    }
}
