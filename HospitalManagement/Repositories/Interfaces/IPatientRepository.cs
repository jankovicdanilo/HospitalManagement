using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task Delete(int id);
    }
}
