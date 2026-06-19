using HospitalManagement.Appointments.Clients.Interfaces;
using HospitalManagement.Appointments.Models.DTOs.External;
using System.Text.Json;

namespace HospitalManagement.Appointments.Clients.Implementations
{
    public class MainApiClient : IMainApiClient
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<MainApiClient> logger;


        public MainApiClient(HttpClient httpClient, ILogger<MainApiClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
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