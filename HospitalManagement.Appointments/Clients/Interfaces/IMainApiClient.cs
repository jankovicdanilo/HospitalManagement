using HospitalManagement.Appointments.Models.DTOs.External;

namespace HospitalManagement.Appointments.Clients.Interfaces
{
    public interface IMainApiClient
    {
        Task<ExternalDoctorDto?> GetDoctorAsync(int doctorId);
        Task<ExternalPatientDto?> GetPatientAsync(int patientId);
        Task<ExternalProcedureDto?> GetProcedureAsync(int procedureId);
        Task<ExternalDoctorScheduleDto?> GetDoctorScheduleAsync(int doctorId, DayOfWeek dayOfWeek);
    }
}