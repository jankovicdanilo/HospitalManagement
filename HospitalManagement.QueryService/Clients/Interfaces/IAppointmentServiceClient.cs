using HospitalManagement.QueryService.Models.DTOs.Patient;

namespace HospitalManagement.QueryService.Clients.Interfaces
{
    public interface IAppointmentServiceClient
    {
        Task<PatientMedicalHistoryDto?> GetPatientHistoryAsync(int patientId);
    }
}
