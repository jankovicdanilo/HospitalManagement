using AutoMapper;
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
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IMapper mapper;
        private readonly AppointmentValidation appointmentValidation;

        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper, AppointmentValidation appointmentValidation)
        {
            this.appointmentRepository = appointmentRepository;
            this.mapper = mapper;
            this.appointmentValidation = appointmentValidation;
        }

        public async Task<Result<List<AppointmentListResponseDto>>> GetAllAsync()
        {
            var appointments = await appointmentRepository.GetAllAsync();
            var result = mapper.Map<List<AppointmentListResponseDto>>(appointments);
            return Result<List<AppointmentListResponseDto>>.Ok(result);
        }

        public async Task<Result<AppointmentResponseDto>> GetByIdAsync(int id)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(id);
            if (appointmentDomain == null)
                return Result<AppointmentResponseDto>.Fail($"Appointment with the id {id} not found", "INVALID_ID");

            var result = mapper.Map<AppointmentResponseDto>(appointmentDomain);
            return Result<AppointmentResponseDto>.Ok(result);
        }

        public async Task<Result<CreateAppointmentResponseDto>> CreateAsync(CreateAppointmentRequestDto request)
        {
            var validate = await appointmentValidation.ValidateAll(request.DoctorId, request.PatientId, request.DateTime, request.Duration);
            if (!validate.Success)
                return Result<CreateAppointmentResponseDto>.Fail(validate.Message, validate.ErrorCode);

            var appointmentDomain = mapper.Map<Appointment>(request);
            appointmentDomain = await appointmentRepository.CreateAsync(appointmentDomain);

            var result = mapper.Map<CreateAppointmentResponseDto>(appointmentDomain);
            return Result<CreateAppointmentResponseDto>.Ok(result);
        }

        public async Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request)
        {
            var validatedAppointment = await appointmentValidation.ValidateAll(request.Id, request.DoctorId, request.PatientId, request.DateTime, request.Duration);
            if (!validatedAppointment.Success)
                return Result<AppointmentUpdateResponseDto>.Fail(validatedAppointment.Message, validatedAppointment.ErrorCode);

            var appointmentDomain = validatedAppointment.Data;
            mapper.Map(request, appointmentDomain);
            appointmentDomain = await appointmentRepository.UpdateAsync(appointmentDomain);

            var result = mapper.Map<AppointmentUpdateResponseDto>(appointmentDomain);
            return Result<AppointmentUpdateResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var appointmentDomain = await appointmentRepository.Delete(id);
            if (appointmentDomain == null)
                return Result.Fail($"Appointment with the id {id} not found", "INVALID_ID");

            return Result.Ok("Appointment deleted");
        }
    }
}