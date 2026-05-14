using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<List<Appointment>> GetAllAsync();

        Task<Appointment?> GetByIdAsync(int id);
    }
}
