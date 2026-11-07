using HospitalManagement.QueryService.Models.DTOs.Patient;

namespace HospitalManagement.QueryService.Clients
{
    public class HistoryFetchResult
    {
        public HistoryFetchStatus Status { get; init; }
        public string? Message { get; init; }
        public string? ErrorCode { get; init; }
        public PatientMedicalHistoryDto? History { get; init; }
    }
}
