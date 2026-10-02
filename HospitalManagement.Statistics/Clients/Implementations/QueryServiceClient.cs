using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.DTOs.Patient;
using HospitalManagement.Statistics.Clients.Interfaces;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HospitalManagement.Statistics.Clients.Implementations
{
    public class QueryServiceClient : IQueryServiceClient
    {
        private readonly HttpClient httpClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public QueryServiceClient(HttpClient httpClient) => this.httpClient = httpClient;

        public async Task<DoctorResponseDto?> GetDoctorAsync(int doctorId)
        {
            var response = await httpClient.GetAsync($"api/doctor/{doctorId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DoctorResponseDto>(JsonOptions);
        }

        public async Task<PatientResponseDto?> GetPatientAsync(int patientId)
        {
            var response = await httpClient.GetAsync($"api/patient/{patientId}");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PatientResponseDto>(JsonOptions);
        }
    }
}
