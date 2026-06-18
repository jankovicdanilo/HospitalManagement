using HospitalManagement.Appointments.Models.Domain;

namespace HospitalManagement.Appointments.Repositories.Interfaces
{
    public interface IAppointmentProcedureRepository
    {
        Task<AppointmentProcedure?> GetByAppointmentAndProcedureIdAsync(int appointmentId, int procedureId);
        Task<AppointmentProcedure?> CreateAsync(AppointmentProcedure request);
        Task<AppointmentProcedure?> DeleteAsync(int appointmentId, int procedureId);
    }
}