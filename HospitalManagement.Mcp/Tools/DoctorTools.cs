using HospitalManagement.Mcp.Api;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace HospitalManagement.Mcp.Tools
{
    [McpServerToolType]
    public class DoctorTools(HospitalApi hospitalApi, ILogger<DoctorTools> logger)
    {
        [McpServerTool(Name = "get_doctor", ReadOnly = true), Description("Gets one doctor by id.")]
        public Task<string> GetDoctor([Description("Doctor id")] int id)
        {
            logger.LogInformation("get_doctor {DoctorId}", id);
            return hospitalApi.GetAsync(ApiNames.Query, $"api/doctor/{id}");
        }

        [McpServerTool(Name = "search_doctors", ReadOnly = true),
            Description("Searches doctors by text (name or specialization) and returns one page of results.")]
        public Task<string> SearchDoctors(
            [Description("Text to search for; ommit to list all doctors")] string? search = null,
            [Description("Page number, starting at 1")] int pageNumber = 1,
            [Description("Page size")] int pageSize = 20)
        {
            logger.LogInformation("search_doctors page {Page} size {Size}", pageNumber, pageSize);
            return hospitalApi.GetAsync(ApiNames.Query, "api/doctor" + HospitalApi.BuildQuery(
                ("search", search), ("pageNumber", pageNumber), ("pageSize", pageSize)));
        }

        [McpServerTool(Name = "create_doctor"), Description("Creates a doctor.")]
        public Task<string> CreateDoctor(
            [Description("First name")] string firstName,
            [Description("Last name")] string lastName,
            [Description("Specialization, e.g. Cardiology")] string specialization,
            [Description("Email address")] string email,
            [Description("Phone number")] string? phone = null)
        {
            logger.LogInformation("create_doctor requested");
            return hospitalApi.PostAsync(ApiNames.Command, "api/doctor",
                new {firstName, lastName, specialization, email, phone});
        }

        [McpServerTool(Name = "delete_doctor", Destructive = true), 
            Description("Permanently deletes a doctor. Only pass confirm=true after the user has explicitly asked to delete this doctor")]
        public Task<string> DeleteDoctor(
            [Description("Doctor id")] int id,
            [Description("Must be true to actually delete")] bool confirm = false)
        {
            if (!confirm)
            {
                logger.LogWarning("delete_doctor {DoctorId} refused: confirm was not set", id);
                return Task.FromResult($"Not deleted. Deleting doctor {id} cannot be undone; call again with confirm=true to proceed.");
            }
            
            logger.LogInformation("delete_doctor {DoctorId} confirmed", id);
            return hospitalApi.DeleteAsync(ApiNames.Command, $"api/doctor/{id}");
        }
    }
}
