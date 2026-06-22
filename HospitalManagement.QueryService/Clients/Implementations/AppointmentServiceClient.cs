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
                    return null;
                }
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<List<AppointmentResponseDto>>>(json, JsonOptions);

                if(result?.Data == null)
                {
                    return null;
                }

                return new PatientMedicalHistoryDto
                {
                    PatientId = patientId,
                    Appointments = result.Data.Select(a => new AppointmentHistoryDto
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

        internal class ApiResponse<T>
        {
            public T? Data { get; set; }
            public bool Success { get; set; }
            public string? Message { get; set; }
        }
    }
}
