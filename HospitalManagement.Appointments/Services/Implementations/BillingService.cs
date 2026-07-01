using AutoMapper;
using HospitalManagement.Appointments.Models.DTOs.Appointment;
using HospitalManagement.Appointments.Models.DTOs.Invoice;
using HospitalManagement.Appointments.Services.Interfaces;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.Appointments.Services.Implementations
{
    public class BillingService : IBillingService
    {
        private readonly IAppointmentService appointmentsService;
        private readonly IPdfGenerator pdfGenerator;
        private readonly ILogger<BillingService> logger;
        private readonly IMapper mapper;

        public BillingService(IAppointmentService appointmentsService, 
            IPdfGenerator pdfGenerator, ILogger<BillingService> logger, IMapper mapper)
        {
            this.appointmentsService = appointmentsService;
            this.pdfGenerator = pdfGenerator;
            this.logger = logger;
            this.mapper = mapper;

        }

        public async Task<Result<InvoiceResult>> GenerateInvoiceAsync(int appointmentId)
        {
            var appointment = await appointmentsService.GetByIdAsync(appointmentId);

            if (!appointment.Success)
            {
                logger.LogWarning("Invoice generation failed - appointment with id {Id} not found", appointmentId);
                return Result<InvoiceResult>.Fail(appointment.Message, appointment.ErrorCode);
            }

            if (appointment.Data.Patient == null || appointment.Data.Doctor == null)
            {
                logger.LogWarning("Invoice generation failed - appointment with id {Id} has incomplete data", appointmentId);
                return Result<InvoiceResult>.Fail("Appointment data is incomplete", "INVALID_DATA");
            }

            var invoiceData = mapper.Map<InvoiceData>(appointment.Data);
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
