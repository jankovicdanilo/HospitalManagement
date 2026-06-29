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
                logger.LogWarning("Invoice generation failed - appointment with id {Id} not found", appointmentId);
                return Result<byte[]>.Fail(appointment.Message, appointment.ErrorCode);
            }

            if (appointment.Data.Patient == null || appointment.Data.Doctor == null)
            {
                logger.LogWarning("Invoice generation failed - appointment with id {Id} has incomplete data", appointmentId);
                return Result<byte[]>.Fail("Appointment data is incomplete", "INVALID_DATA");
            }

            var invoiceData = MapToInvoiceData(appointment.Data);
            var pdfBytes = pdfGenerator.Generate(invoiceData);

            logger.LogInformation("Invoice {InvoiceNumber} generated successfully for appointment {Id}",
                invoiceData.InvoiceNumber, appointmentId);

            return Result<byte[]>.Ok(pdfBytes);
        }

        private InvoiceData MapToInvoiceData(AppointmentResponseDto appointment)
        {
            return new InvoiceData
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyy}-{appointment.Id:D5}",
                IssuedDate = DateTime.UtcNow,
                PatientName = $"{appointment.Patient.Name} {appointment.Patient.LastName}",
                DoctorName = $"{appointment.Doctor.FirstName} {appointment.Doctor.LastName}",
                AppointmentDate = appointment.DateTime,
                Status = appointment.Status,
                Duration = appointment.Duration,
                Notes = appointment.Notes,
                Procedures = appointment.Procedures.Select(p => new InvoiceLineItem
                {
                    Name = p.ProcedureName,
                    Price = p.ProcedurePrice
                }).ToList(),
                Subtotal = appointment.Procedures.Sum(p => p.ProcedurePrice),
                Discount = appointment.Discount,
                TotalAmount = appointment.TotalCost
            };
        }
    }
}
