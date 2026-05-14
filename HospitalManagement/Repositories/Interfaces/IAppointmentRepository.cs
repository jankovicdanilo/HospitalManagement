using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> Delete(int id);

        Task<Appointment?> GetByIdAsync(int id); 
    }
}
