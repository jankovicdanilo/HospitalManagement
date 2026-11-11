using HospitalManagement.QueryService.Models.DTOs.Patient;

namespace HospitalManagement.QueryService.Clients.Interfaces
{
    public interface IAppointmentServiceClient
    {
        Task<HistoryFetchResult> GetPatientHistoryAsync(int patientId);
        Task<List<int>?> GetPopularDoctorIdsAsync(int count);
        Task<List<int>?> GetPopularPatientIdsAsync(int count);
    }
}
