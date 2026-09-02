using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.QueryService.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<(List<Doctor> items, int totalCount)> GetAllAsync(DoctorFilterDto filter);
        Task<Doctor?> GetByIdAsync(int id);
    }
}