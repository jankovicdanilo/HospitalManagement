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
                return Result<byte[]>.Fail(appointment.Message, appointment.ErrorCode);
            }

            throw new NotImplementedException();
        }

        public Task<Result<InvoiceData>> GetInvoiceDataAsync(int appointmentId)
        {
            throw new NotImplementedException();
        }
    }
}
