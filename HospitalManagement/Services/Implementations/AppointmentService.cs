using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Services.Validations;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository apointmentRepository;
        private readonly AppointmentValidation appointmentValidation;

        public AppointmentService(IAppointmentRepository apointmentRepository, AppointmentValidation appointmentValidation)
        {
            this.apointmentRepository = apointmentRepository;
            this.appointmentValidation = appointmentValidation;
        }

        public async Task<Result<CreateAppointmentResponseDto>> CreateAsync(CreateAppointmentRequestDto request)
        {
            var validate = await appointmentValidation.ValidateAll(request.DoctorId, request.PatientId, request.DateTime);

            if (!validate.Success)
            {
                return Result<CreateAppointmentResponseDto>.Fail(validate.Message, validate.ErrorCode);
            }

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
