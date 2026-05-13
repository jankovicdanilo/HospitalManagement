using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsync();

        Task<Appointment?> GetByIdAsync(int id);
    }
}
