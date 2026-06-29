using HospitalManagement.Appointments.Services.Implementations;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Appointments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : BaseController
    {
        private readonly IBillingService billingService;

        public BillingController(IBillingService billingService)
        {
            this.billingService = billingService;
        }

        [HttpGet("{appointmentId:int}/pdf")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateInvoiceAsync(int appointmentId)
        {
            var result = await billingService.GenerateInvoiceAsync(appointmentId);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return File(result.Data, "application/pdf", $"invoice_{appointmentId}.pdf");
        }
    }
}
