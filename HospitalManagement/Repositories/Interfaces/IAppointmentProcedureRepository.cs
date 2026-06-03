using HospitalManagement.Models.Domain;

namespace HospitalManagement.Repositories.Interfaces
{
    public interface IAppointmentProcedureRepository
    {
        Task<AppointmentProcedure?> GetAsync(int appointmentId, int procedureId);
        Task<AppointmentProcedure?> AddAsync(AppointmentProcedure request);
        Task<AppointmentProcedure?> RemoveAsync(int appointmentId, int procedureId);
    }
}
