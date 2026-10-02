using HospitalManagement.Shared.Controllers;
using HospitalManagement.Statistics.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Statistics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : BaseController
    {
        private readonly IStatisticsService statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
            => this.statisticsService = statisticsService;

        [HttpGet("doctors/load/timeline")]
        public async Task<IActionResult> GetDoctorsLoadTimelineAsync(
    [FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] int top = 10)
        {
            var result = await statisticsService.GetDoctorsLoadTimelineAsync(from, to, top);
            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("doctors/{doctorId:int}")]
        public async Task<IActionResult> GetDoctorStatisticsAsync(
            [FromRoute] int doctorId, [FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var result = await statisticsService.GetDoctorStatisticsAsync(doctorId, from, to);
            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("doctors/revenue")]
        public async Task<IActionResult> GetDoctorsRevenueAsync([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var result = await statisticsService.GetDoctorsRevenueAsync(from, to);
            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("doctors/load")]
        public async Task<IActionResult> GetDoctorsLoadAsync([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var result = await statisticsService.GetDoctorsLoadAsync(from, to);
            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("procedures/profitability")]
        public async Task<IActionResult> GetProceduresProfitabilityAsync([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var result = await statisticsService.GetProceduresProfitabilityAsync(from, to);
            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatientStatisticsAsync([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var result = await statisticsService.GetPatientStatisticsAsync(from, to);
            return result.Success ? Ok(result.Data) : HandleFailure(result);
        }
    }
}
