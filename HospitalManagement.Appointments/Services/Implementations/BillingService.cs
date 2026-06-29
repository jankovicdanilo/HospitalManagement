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

        public BillingService(IAppointmentService appointmentsService, 
            IPdfGenerator pdfGenerator, ILogger<BillingService> logger)
        {
            this.appointmentsService = appointmentsService;
            this.pdfGenerator = pdfGenerator;
            this.logger = logger;
        }

        public async Task<Result<byte[]>> GenerateInvoiceAsync(int appointmentId)
        {
            var appointment = await appointmentsService.GetByIdAsync(appointmentId);

            if (!appointment.Success)
            {
                logger.LogWarning("Appointment with id {Id} not found", appointmentId);
                return Result<byte[]>.Fail(appointment.Message, appointment.ErrorCode);
            }

            var invoiceData = MapToInvoiceData(appointment.Data);
            var pdfBytes = pdfGenerator.Generate(invoiceData);

            return Result<byte[]>.Ok(pdfBytes);
        }

        private InvoiceData MapToInvoiceData(AppointmentResponseDto appointment)
        {
            return new InvoiceData();
        }
    }
}
