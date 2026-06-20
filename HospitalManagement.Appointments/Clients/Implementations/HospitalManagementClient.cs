using HospitalManagement.Appointments.Clients.Interfaces;
using HospitalManagement.Shared.Models.DTOs;
using System.Text.Json;

namespace HospitalManagement.Appointments.Clients.Implementations
{
    public class HospitalManagementClient : IHospitalManagementClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<HospitalManagementClient> logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = {new System.Text.Json.Serialization.JsonStringEnumConverter()}
        };

        public HospitalManagementClient(HttpClient httpClient, ILogger<HospitalManagementClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<DoctorResponseDto?> GetDoctorAsync(int doctorId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/doctor/{doctorId}");
                if (!response.IsSuccessStatusCode)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response body: {Json}", json);
                var result = JsonSerializer.Deserialize<ApiResponse<DoctorResponseDto>>(json, JsonOptions);

                return result?.Data;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to get doctor {DoctorId} from main API", doctorId);
                return null;
            }
        }

        public async Task<PatientResponseDto?> GetPatientAsync(int patientId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/patient/{patientId}");
                if (!response.IsSuccessStatusCode)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response body: {Json}", json);
                var result = JsonSerializer.Deserialize<ApiResponse<PatientResponseDto>>(json, JsonOptions);

                return result?.Data;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Failed to get patient {PatientId} from main API", patientId);
                return null;
            }
        }

        public async Task<ProcedureResponseDto?> GetProcedureAsync(int procedureId)
        //{
        //    try
        //    {
        //        var response = await httpClient.GetAsync($"api/procedure/{procedureId}");

        //            return null;

        //        var result = JsonSerializer.Deserialize<ApiResponse<ExternalProcedureDto>>(json, JsonOptions);

        //        return result?.Data;
                var result = JsonSerializer.Deserialize<ApiResponse<ProcedureResponseDto>>(json, JsonOptions);
                logger.LogInformation("Deserialized data: {Data}", result?.Data?.Name);
        //    {
        //        logger.LogError(ex, "Failed to get procedure {ProcedureId} from main API", procedureId);
        //        return null;
        //    }
        //}

        public async Task<ExternalProcedureDto?> GetProcedureAsync(int procedureId)
        {
            try
            {
                var url = $"api/procedure/{procedureId}";
                logger.LogInformation("Calling main API: {BaseAddress}{Url}", httpClient.BaseAddress, url);

                var response = await httpClient.GetAsync(url);
                logger.LogInformation("Response status: {StatusCode}", response.StatusCode);

                var json = await response.Content.ReadAsStringAsync();
                logger.LogInformation("Response body: {Json}", json);

                if (!response.IsSuccessStatusCode) return null;
                var result = JsonSerializer.Deserialize<ApiResponse<ExternalProcedureDto>>(json, JsonOptions);
                logger.LogInformation("Deserialized data: {Data}", result?.Data?.Name);
                return result?.Data;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get procedure {ProcedureId} from main API", procedureId);
                return null;
            }
        }

        public async Task<DoctorScheduleResponseDto?> GetDoctorScheduleAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/doctorschedule/doctor/{doctorId}/day/{dayOfWeek}");
                if (!response.IsSuccessStatusCode)
                    return null;
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<DoctorScheduleResponseDto>>(json, JsonOptions);

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