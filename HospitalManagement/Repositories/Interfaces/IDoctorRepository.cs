

using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<Doctor?> Create(Doctor request);

        Task<List<Doctor>> GetAll();

        Task<Doctor?> GetById(int id);

        Task<Doctor?> Update(Doctor request);

        Task Delete(int id);
    }
}
