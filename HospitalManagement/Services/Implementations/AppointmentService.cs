using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository apointmentRepository;

        public AppointmentService(IAppointmentRepository apointmentRepository)
        {
            this.apointmentRepository = apointmentRepository;
        }

        public async Task<Result<CreateAppointmentResponseDto>> CreateAsync(CreateAppointmentRequestDto request)
        {
            var appointmentDomain = new Appointment
            {
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                DateTime = request.DateTime,
                Status = request.Status,
                Notes = request.Notes
            };

            appointmentDomain = await apointmentRepository.CreateAsync(appointmentDomain);

            var result = new CreateAppointmentResponseDto
                (
                    appointmentDomain.Id,
                    appointmentDomain.PatientId,
                    appointmentDomain.DoctorId,
                    appointmentDomain.DateTime,
                    appointmentDomain.Status,
                    appointmentDomain.Status
                );

            return Result<CreateAppointmentResponseDto>.Ok(result);
        }
    }
}
