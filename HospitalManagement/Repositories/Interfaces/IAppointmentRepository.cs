using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<List<Appointment>> GetAllAsync();
        Task<Appointment> UpdateAsync(Appointment appointment);

        Task<Appointment?> GetByIdAsync(int id);

        Task<List<Appointment>?> GetByDoctorIdAsync(int id); 
    }
}
