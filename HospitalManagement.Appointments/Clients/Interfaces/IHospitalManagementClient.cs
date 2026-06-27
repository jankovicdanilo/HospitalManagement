using HospitalManagement.Shared.Models.DTOs;

namespace HospitalManagement.Appointments.Clients.Interfaces
{
    public interface IHospitalManagementClient
    {
        Task<DoctorResponseDto?> GetDoctorAsync(int doctorId);
        Task<PatientResponseDto?> GetPatientAsync(int patientId);
        Task<ProcedureResponseDto?> GetProcedureAsync(int procedureId);
        Task<DoctorScheduleResponseDto?> GetDoctorScheduleAsync(int doctorId, DayOfWeek dayOfWeek);
    }
}