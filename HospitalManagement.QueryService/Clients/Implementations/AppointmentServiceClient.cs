using HospitalManagement.QueryService.Clients.Interfaces;
using HospitalManagement.QueryService.Models.DTOs.Patient;
using NLog.Targets;
using System.Text.Json;

namespace HospitalManagement.QueryService.Clients.Implementations
{
    public class AppointmentServiceClient : IAppointmentServiceClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<AppointmentServiceClient> logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AppointmentServiceClient(HttpClient httpClient, ILogger<AppointmentServiceClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<PatientMedicalHistoryDto?> GetPatientHistoryAsync(int patientId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/appointment/patient/{patientId}/history");
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Failed to get patient history for patient {PatientId}, status {StatusCode}",
                        patientId, (int)response.StatusCode);
                    return null;
                }

                var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentResponseDto>>(JsonOptions);
                if (appointments == null)
                {
                    logger.LogWarning("Appointments returned an empty body for the history of patient {PatientId}", patientId);
                    return null;
                }

                logger.LogInformation("Loaded {Count} appointments for the history of patient {PatientId}", appointments.Count, patientId);

                return new PatientMedicalHistoryDto
                {
                    PatientId = patientId,
                    Appointments = appointments.Select(a => new AppointmentHistoryDto
                    {
                        Id = a.Id,
                        DateTime = a.DateTime,
                        Duration = a.Duration,
                        Status = a.Status,
                        Notes = a.Notes,
                        DoctorName = a.DoctorName,
                        TotalCost = a.TotalCost,
                        Discount = a.Discount,
                        Procedures = a.Procedures,
                        Treatment = a.Treatment
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get patient history for patient {PatientId}", patientId);
                return null;
            }
        }

        public async Task<List<int>?> GetPopularDoctorIdsAsync(int count)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/appointment/doctors/popular-ids?count={count}");
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Failed to fetch popular doctor ids, status {Status}", response.StatusCode);
                    return null;
                }

                var ids = await response.Content.ReadFromJsonAsync<List<int>>();
                logger.LogInformation("Deserialized {Count} doctor ids: {Ids}", ids?.Count, string.Join(",", ids ?? new List<int>()));
                return ids;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to get popular doctor ids");
                return null;
            }
            
        }

        public async Task<List<int>?> GetPopularPatientIdsAsync(int count)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/appointment/patients/popular-ids?count={count}");
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("Failed to fetch popular patient ids, status {Status}", response.StatusCode);
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<List<int>>();
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to get popular patient ids");
                return null;
            }
        }
    }
}
