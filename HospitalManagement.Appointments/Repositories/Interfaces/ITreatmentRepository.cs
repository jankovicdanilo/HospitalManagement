using HospitalManagement.Appointments.Models.Domain;

namespace HospitalManagement.Appointments.Repositories.Interfaces
{
    public interface ITreatmentRepository
    {
        Task<Treatment> CreateAsync(Treatment treatment);
        Task<Treatment?> GetByIdAsync(int id);
        Task<bool> TreatmentExists(int appointmentId);
    }
}