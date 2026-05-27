using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface ITreatmentRepository
    {
        Task<Treatment> CreateAsync(Treatment treatment);

        Task<Treatment?> GetByIdAsync(int id);

        Task<bool> TreatmentExists(int appointmentId);
    }
}
