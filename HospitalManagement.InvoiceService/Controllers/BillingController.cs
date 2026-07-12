using HospitalManagement.InvoiceService.Models.Enums;
using HospitalManagement.InvoiceService.Services.Interfaces;
using HospitalManagement.Shared.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.InvoiceService.Controllers
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

        [HttpGet("{appointmentId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateInvoiceAsync(int appointmentId, [FromQuery] string format = "pdf")
        {
            if(!Enum.TryParse<InvoiceFormat>(format, ignoreCase: true, out var invoiceFormat))
            {
                return BadRequest(new { Message = $"Unsupported format '{format}'. Supported formats: pdf, docx.", ErrorCode = "INVALID_FORMAT" });
            }

            var result = await billingService.GenerateInvoiceAsync(appointmentId, invoiceFormat);

            if (!result.Success)
            {
                return NotFound(new { result.Message, result.ErrorCode });
            }

            return File(result.Data!.FileBytes!, result.Data.ContentType!,
                    $"{result.Data.PatientName}_{result.Data.InvoiceNumber}.{result.Data.FileExtension}");
        }
    }
}
