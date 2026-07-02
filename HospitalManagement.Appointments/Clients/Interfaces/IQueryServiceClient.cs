using HospitalManagement.Shared.Models.DTOs.Procedure;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.Appointments.Clients.Interfaces
{
    public interface IQueryServiceClient
    {
        Task<DoctorResponseDto?> GetDoctorAsync(int doctorId);
        Task<PatientResponseDto?> GetPatientAsync(int patientId);
        Task<ProcedureResponseDto?> GetProcedureAsync(int procedureId);
        Task<DoctorScheduleResponseDto?> GetDoctorScheduleAsync(int doctorId, DayOfWeek dayOfWeek);
    }
}