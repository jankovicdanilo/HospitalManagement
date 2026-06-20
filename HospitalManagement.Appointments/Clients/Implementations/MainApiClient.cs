using HospitalManagement.Appointments.Clients.Interfaces;
using HospitalManagement.Appointments.Models.DTOs.External;
using System.Text.Json;

namespace HospitalManagement.Appointments.Clients.Implementations
{
    public class MainApiClient : IMainApiClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<MainApiClient> logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public MainApiClient(HttpClient httpClient, ILogger<MainApiClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<ExternalDoctorDto?> GetDoctorAsync(int doctorId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/doctor/{doctorId}");
                if (!response.IsSuccessStatusCode)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<ExternalDoctorDto>>(json, JsonOptions);

                return result?.Data;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to get doctor {DoctorId} from main API", doctorId);
                return null;
            }
        }

        public async Task<ExternalPatientDto?> GetPatientAsync(int patientId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/patient/{patientId}");
                if (!response.IsSuccessStatusCode)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<ExternalPatientDto>>(json, JsonOptions);

                return result?.Data;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to get patient {PatientId} from main API", patientId);
                return null;
            }
        }

        public async Task<ExternalProcedureDto?> GetProcedureAsync(int procedureId)
        {
            try
            {
                var url = $"api/procedure/{procedureId}";
                var response = await httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) return null;
                var result = JsonSerializer.Deserialize<ApiResponse<ExternalProcedureDto>>(json, JsonOptions);

                return result?.Data;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get procedure {ProcedureId} from main API", procedureId);
                return null;
            }
        }

        public async Task<ExternalDoctorScheduleDto?> GetDoctorScheduleAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/doctorschedule/doctor/{doctorId}/day/{dayOfWeek}");
                if (!response.IsSuccessStatusCode)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<ExternalDoctorScheduleDto>>(json, JsonOptions);

                return result?.Data;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to get schedule for doctor {DoctorId} on {DayOfWeek}", doctorId, dayOfWeek);
                return null;
            }
        }
    }

    // Wrapper matching API's Result<T> response shape
    internal class ApiResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}