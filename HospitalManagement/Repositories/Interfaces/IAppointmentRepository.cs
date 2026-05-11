using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> UpdateAsync(Appointment appointment);

        Task<Appointment?> GetByIdAsync(int id);
    }
}
