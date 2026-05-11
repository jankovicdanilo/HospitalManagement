using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentRepository
    {
        Task Delete(int id);

        Task<Appointment?> GetByIdAsync(int id); 
    }
}
