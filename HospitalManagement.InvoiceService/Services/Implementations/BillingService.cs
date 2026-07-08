using AutoMapper;
using HospitalManagement.InvoiceService.Clients.Interfaces;
using HospitalManagement.InvoiceService.Models.DTOs.Invoice;
using HospitalManagement.InvoiceService.Services.Interfaces;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.InvoiceService.Services.Implementations
{
    public class BillingService : IBillingService
    {
        private readonly IAppointmentServiceClient appointmentService;
        private readonly IPdfGenerator pdfGenerator;
        private readonly ILogger<BillingService> logger;
        private readonly IMapper mapper;

        public BillingService(IAppointmentServiceClient appointmentService, 
            IPdfGenerator pdfGenerator, ILogger<BillingService> logger, IMapper mapper)
        {
            this.appointmentService = appointmentService;
            this.pdfGenerator = pdfGenerator;
            this.logger = logger;
            this.mapper = mapper;

        }

        public async Task<Result<InvoiceResult>> GenerateInvoiceAsync(int appointmentId)
        {
            var appointment = await appointmentService.GetAppointmentAsync(appointmentId);

            if (appointment == null)
            {
                logger.LogWarning("Invoice generation failed - appointment with id {Id} not found", appointmentId);
                return Result<InvoiceResult>.Fail($"Appointment with the id {appointmentId} not found", "INVALID_ID");
            }

            if (appointment.Patient == null || appointment.Doctor == null)
            {
                logger.LogWarning("Invoice generation failed - appointment with id {Id} has incomplete data", appointmentId);
                return Result<InvoiceResult>.Fail("Appointment data is incomplete", "INVALID_DATA");
            }

            var invoiceData = mapper.Map<InvoiceData>(appointment);
            var pdfBytes = pdfGenerator.Generate(invoiceData);
            var invoiceResult = new InvoiceResult
            {
                PdfBytes = pdfBytes,
                PatientName = invoiceData.PatientName,
                InvoiceNumber = invoiceData.InvoiceNumber
            };

            logger.LogInformation("Invoice {InvoiceNumber} generated successfully for appointment {Id}",
                invoiceData.InvoiceNumber, appointmentId);

            return Result<InvoiceResult>.Ok(invoiceResult);
        }
    }
}
