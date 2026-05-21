using AutoMapper;
using Azure.Core;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Services.Validations;
using Microsoft.Extensions.Options;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IMapper mapper;
        private readonly AppointmentValidation appointmentValidation;
        private readonly AppointmentSettings appointmentSettings;
        private readonly ILogger<AppointmentService> logger;

        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper,
            AppointmentValidation appointmentValidation, IOptions<AppointmentSettings> appointmentSettings, ILogger<AppointmentService> logger)
        {
            this.appointmentRepository = appointmentRepository;
            this.mapper = mapper;
            this.appointmentValidation = appointmentValidation;
            this.appointmentSettings = appointmentSettings.Value;
            this.logger = logger;
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
            {
                logger.LogError("Appointment with id {id} not found", id);
                return Result<AppointmentResponseDto>.Fail($"Appointment with the id {id} not found", "INVALID_ID");
            }
                
            var result = mapper.Map<AppointmentResponseDto>(appointmentDomain);

            return Result<AppointmentResponseDto>.Ok(result);
        }

        public async Task<Result<AppointmentCreateResponseDto>> CreateAsync(AppointmentCreateRequestDto request)
        {
            var validatedAppointment = await appointmentValidation.ValidateAll(request);

            if (!validatedAppointment.Success)
            {
                logger.LogError("Appointment creation failed: {Message}", validatedAppointment.Message);
                return Result<AppointmentCreateResponseDto>.Fail(validatedAppointment.Message, validatedAppointment.ErrorCode);
            }

            var appointmentDomain = mapper.Map<Appointment>(request);

            appointmentDomain = await appointmentRepository.CreateAsync(appointmentDomain);

            logger.LogInformation("Appointment created with id {id}", appointmentDomain.Id);

            var result = mapper.Map<AppointmentCreateResponseDto>(appointmentDomain);

            return Result<AppointmentCreateResponseDto>.Ok(result);
        }

        public async Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request)
        {
            var validatedAppointment = await appointmentValidation.ValidateAll(request);

            if (!validatedAppointment.Success)
            {
                logger.LogError("Appointment update failed : {Message}", validatedAppointment.Message);
                return Result<AppointmentUpdateResponseDto>.Fail(validatedAppointment.Message,
                    validatedAppointment.ErrorCode);
            }
                
            var appointmentDomain = await appointmentRepository.GetByIdAsync(request.Id);
            
            if (appointmentDomain == null)
            {
                logger.LogError("Appointment with id {Id} not found", request.Id);
                return Result<AppointmentUpdateResponseDto>.Fail($"Appointment with the id {request.Id} not found", 
                    "INVALID_ID");
            }

            mapper.Map(request, appointmentDomain);

            appointmentDomain = await appointmentRepository.UpdateAsync(appointmentDomain);

            logger.LogInformation("Appointment with id {Id} updated", appointmentDomain.Id);

            var result = mapper.Map<AppointmentUpdateResponseDto>(appointmentDomain);

            return Result<AppointmentUpdateResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var appointmentDomain = await appointmentRepository.Delete(id);

            if (appointmentDomain == null)
            {
                logger.LogError("Appointment with id {Id} not found for deletion", id);
                return Result.Fail($"Appointment with the id {id} not found", "INVALID_ID");
            }

            logger.LogInformation("Appointment with id {Id} deleted", id);
            return Result.Ok("Appointment deleted");
        }

        public async Task<Result<List<TimeSlotDto>>> GetFreeSlotsAsync(int doctorId, DateOnly date)
        {
            var workStart = new TimeSpan(appointmentSettings.WorkStartHour, 0, 0);
            var workEnd = new TimeSpan(appointmentSettings.WorkEndHour, 0, 0);
            var slotSize = new TimeSpan(0, appointmentSettings.SlotSizeMinutes, 0);

            var appointments = await appointmentRepository.GetByDoctorIdAndDateAsync(doctorId, date);

            var freeSlots = new List<TimeSlotDto>();
            var current = workStart;

            while (current + slotSize <= workEnd)
            {
                var slotStart = date.ToDateTime(TimeOnly.FromTimeSpan(current));
                var slotEnd = slotStart.Add(slotSize);

                var isBooked = appointments.Any(a =>
                    slotStart < a.DateTime.Add(a.Duration) &&
                    slotEnd > a.DateTime);

                if (!isBooked)
                {
                    freeSlots.Add(new TimeSlotDto
                    {
                        Start = TimeOnly.FromDateTime(slotStart),
                        End = TimeOnly.FromDateTime(slotEnd)
                    });
                }

                current = current.Add(slotSize);
            }

            return Result<List<TimeSlotDto>>.Ok(freeSlots);
        }
    }
}