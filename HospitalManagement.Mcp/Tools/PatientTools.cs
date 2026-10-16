using HospitalManagement.Mcp.Api;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace HospitalManagement.Mcp.Tools
{
    [McpServerToolType]
    public class PatientTools(HospitalApi hospitalApi, ILogger<PatientTools> logger)
    {
        [McpServerTool(Name = "get_patient", ReadOnly = true), Description("Gets one patient by id")]
        public Task<string> GetPatient([Description("Patient id")] int id)
        {
            logger.LogInformation("get_patient {PatientId}", id);
            return hospitalApi.GetAsync(ApiNames.Query, $"api/patient/{id}");
        }

        [McpServerTool(Name = "search_patients", ReadOnly = true),
            Description("Searches patients by text (name or email) and returns one page of results")]
        public Task<string> SearchPatients(
            [Description("Text to search for; omit to list all patients")] string? search = null,
            [Description("Page number, starting at 1")] int pageNumber = 1,
            [Description("Page size")] int pageSize = 20)
        {
            logger.LogInformation("search_patients page {Page} size {Size}", pageNumber, pageSize);
            return hospitalApi.GetAsync(ApiNames.Query, "api/patient" + HospitalApi.BuildQuery(
                ("search", search), ("pageNumber", pageNumber), ("pageSize", pageSize)));
        }

        [McpServerTool(Name = "create_patient"), Description("Creates a patient.")]
        public Task<string> CreatePatient(
            [Description("First name")] string name,
            [Description("Last name")] string lastName,
            [Description("Email address")] string email,
            [Description("Date of birth, yyyy-MM-dd")] string dateOfBirth,
            [Description("Phone number")] string? phone = null)
        {
            logger.LogInformation("create_patient requested");
            return hospitalApi.PostAsync(ApiNames.Command, "api/patient",
                new {name,  lastName, email, dateOfBirth, phone});
        }

        [McpServerTool(Name = "delete_patient", Destructive = true),
            Description("Permanently deletes a patient. Only pass confim=true after the user " +
            "had explicitly asked to delete this patient")]
        public Task<string> DeletePatient(
            [Description("Patient id")]int id,
            [Description("Must be true to actually delete")]bool confirm = false)
        {
            if (!confirm)
            {
                logger.LogWarning("delete_patient {PatientId} refused: confirm was not set.", id);
                return Task.FromResult($"Not deleted. Deleting patient {id} cannot be undone; call again with confirm=true to proceed.");
            }

            logger.LogInformation("delete_patient {PatientId} confirmed", id);
            return hospitalApi.DeleteAsync(ApiNames.Command, $"api/patient/{id}");
        }

    }
}
