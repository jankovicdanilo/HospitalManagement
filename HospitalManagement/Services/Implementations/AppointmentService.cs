using HospitalManagement.Common;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Services.Validations;
using AutoMapper;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly AppointmentValidation appointmentValidation;

        public AppointmentService(IAppointmentRepository apointmentRepository, AppointmentValidation appointmentValidation)
        {
            this.appointmentRepository = apointmentRepository;
            this.appointmentValidation = appointmentValidation;
        }

        public async Task<Result<List<AppointmentListResponseDto>>> GetAllAsync()
        {
            var appointments = await appointmentRepository.GetAllAsync();

            var result = new List<AppointmentListResponseDto>();

            foreach(var appointment in appointments)
            {
                result.Add(new AppointmentListResponseDto
                    (
                        appointment.Id,
                        appointment.PatientId,
                        appointment.DoctorId,
                        appointment.DateTime,
                        appointment.Duration,
                        appointment.Status,
                        appointment.Notes
                    ));
            }

            return Result<List<AppointmentListResponseDto>>.Ok(result);
        }

        public async Task<Result<AppointmentResponseDto>> GetByIdAsync(int id)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(id);

            if (appointmentDomain == null)
            {
                return Result<AppointmentResponseDto>.Fail($"Appointment with the id {id} not found", "INVALID_ID");
            }

            var result = new AppointmentResponseDto
                (
                    appointmentDomain.Id,
                    appointmentDomain.PatientId,
                    appointmentDomain.DoctorId,
                    appointmentDomain.DateTime,
                    appointmentDomain.Duration,
                    appointmentDomain.Status,
                    appointmentDomain.Notes
                );

            return Result<AppointmentResponseDto>.Ok(result);

        }

        public async Task<Result<CreateAppointmentResponseDto>> CreateAsync(CreateAppointmentRequestDto request)
        {
            var validate = await appointmentValidation.ValidateAll(request.DoctorId, request.PatientId, request.DateTime, request.Duration);

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

            appointmentDomain = await appointmentRepository.CreateAsync(appointmentDomain);

            var result = new CreateAppointmentResponseDto
                (
                    appointmentDomain.Id,
                    appointmentDomain.PatientId,
                    appointmentDomain.DoctorId,
                    appointmentDomain.DateTime,
                    appointmentDomain.Duration,
                    appointmentDomain.Status,
                    appointmentDomain.Status
                );

            return Result<CreateAppointmentResponseDto>.Ok(result);
        }
    }
}
