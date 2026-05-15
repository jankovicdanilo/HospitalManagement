using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> UpdateAsync(Appointment appointment);

        Task<Appointment?> GetByIdAsync(int id);

        Task<List<Appointment>?> GetByDoctorIdAsync(int id); 

        Task<List<Appointment>> GetAllAsync();

        Task<List<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateOnly date);
    }
}
