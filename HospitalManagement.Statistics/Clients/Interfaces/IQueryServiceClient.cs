using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Patient;

namespace HospitalManagement.Statistics.Clients.Interfaces
{
    public interface IQueryServiceClient
    {
        Task<DoctorResponseDto?> GetDoctorAsync(int doctorId); // null only when 404
        Task<PatientResponseDto?> GetPatientAsync(int patientId);
    }
}
