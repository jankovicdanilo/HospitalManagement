using AutoMapper;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.Appointment;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Services.Validations;
using Microsoft.Extensions.Options;
using HospitalManagement.Models.Enums;

namespace HospitalManagement.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;
        private readonly IMapper mapper;
        private readonly IAppointmentValidation appointmentValidation;
        private readonly ILogger<AppointmentService> logger;
        private readonly IPatientRepository patientRepository;
        private readonly IDoctorRepository doctorRepository;
        private readonly IDoctorScheduleRepository doctorScheduleRepository;
        private readonly AppointmentSettings appointmentSettings;

        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper,
            IAppointmentValidation appointmentValidation, ILogger<AppointmentService> logger,
            IPatientRepository patientRepository, IDoctorRepository doctorRepository, IDoctorScheduleRepository doctorScheduleRepository,
            IOptions<AppointmentSettings> appointmentSettings)
        {
            this.appointmentRepository = appointmentRepository;
            this.mapper = mapper;
            this.appointmentValidation = appointmentValidation;
            this.logger = logger;
            this.patientRepository = patientRepository;
            this.doctorRepository  = doctorRepository;
            this.doctorScheduleRepository = doctorScheduleRepository;
            this.appointmentSettings = appointmentSettings.Value;
        }

        public async Task<Result<PagedResult<AppointmentListResponseDto>>> GetAllAsync(AppointmentFilterDto filter)
        {
            var (items, totalCount) = await appointmentRepository.GetAllAsync(filter);

            var mapped = mapper.Map<List<AppointmentListResponseDto>>(items);

            var pagedResult = new PagedResult<AppointmentListResponseDto>
            {
                Items = mapped,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return Result<PagedResult<AppointmentListResponseDto>>.Ok(pagedResult);
        }

        public async Task<Result<AppointmentResponseDto>> GetByIdAsync(int id)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(id);
            if (appointmentDomain == null)
            {
                logger.LogWarning("Appointment with id {id} not found", id);
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
                logger.LogWarning("Appointment creation failed: {Message}", validatedAppointment.Message);
                return Result<AppointmentCreateResponseDto>.Fail(validatedAppointment.Message, validatedAppointment.ErrorCode);
            }

            var appointmentDomain = mapper.Map<Appointment>(request);

            appointmentDomain = await appointmentRepository.CreateAsync(appointmentDomain);
            var patient = await patientRepository.GetByIdAsync(request.PatientId);
            var doctor = await doctorRepository.GetByIdAsync(request.DoctorId);

            logger.LogInformation("Appointment created with id {id}", appointmentDomain.Id);
            logger.LogInformation("Email sent to {Email}: Appointment confirmed for {DateTime} with Dr. {Doctor}",
                patient.Email, appointmentDomain.DateTime, $"{doctor.FirstName} {doctor.LastName}");

            var result = mapper.Map<AppointmentCreateResponseDto>(appointmentDomain);

            return Result<AppointmentCreateResponseDto>.Ok(result);
        }

        public async Task<Result<AppointmentUpdateResponseDto>> UpdateAsync(AppointmentUpdateRequestDto request)
        {
            var validatedAppointment = await appointmentValidation.ValidateAll(request);

            if (!validatedAppointment.Success)
            {
                logger.LogWarning("Appointment update failed : {Message}", validatedAppointment.Message);
                return Result<AppointmentUpdateResponseDto>.Fail(validatedAppointment.Message,
                    validatedAppointment.ErrorCode);
            }

            var appointmentDomain = await appointmentRepository.GetByIdAsync(request.Id);

            if (appointmentDomain == null)
            {
                logger.LogWarning("Appointment with id {Id} not found", request.Id);
                return Result<AppointmentUpdateResponseDto>.Fail($"Appointment with the id {request.Id} not found",
                    "INVALID_ID");
            }

            if (appointmentDomain.Status != AppointmentStatus.Pending)
            {
                logger.LogWarning("Appointment with id {Id} cannot be updated, status is {Status}", request.Id, appointmentDomain.Status);
                return Result<AppointmentUpdateResponseDto>.Fail(
                    $"Only pending appointments can be updated",
                    "INVALID_STATUS");
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
                logger.LogWarning("Appointment with id {Id} not found for deletion", id);
                return Result.Fail($"Appointment with the id {id} not found", "INVALID_ID");
            }

            logger.LogInformation("Appointment with id {Id} deleted", id);
            return Result.Ok("Appointment deleted");
        }

        public async Task<Result<List<TimeSlotDto>>> GetFreeSlotsAsync(int doctorId, DateOnly date)
        {
            var doctorSchedule = await doctorScheduleRepository.GetByDoctorIdAndDayAsync(doctorId, date.DayOfWeek);

            if(doctorSchedule == null)
            {
                logger.LogWarning("Doctor {DoctorId} does not work on {DayOfWeek}", doctorId, date.DayOfWeek.ToString());
                return Result<List<TimeSlotDto>>.Fail($"Doctor does not work on {date.DayOfWeek}", "DOCTOR_NOT_AVAILABLE");
            }

            if(date < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                logger.LogWarning("Free slots requested for past date {Date}", date);
                return Result<List<TimeSlotDto>>.Fail("Cannot get free slots for a past date", "INVALID_DATE");
            }

            var workStart = new TimeSpan(doctorSchedule.StartHour, 0, 0);
            var workEnd = new TimeSpan(doctorSchedule.EndHour, 0, 0);
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

        public async Task<Result> UpdateStatusAsync(AppointmentStatusUpdateDto request)
        {
            var appointmentDomain = await appointmentRepository.GetByIdAsync(request.Id);

            if(appointmentDomain == null)
            {
                logger.LogWarning("Appointment with id {Id} not found", request.Id);
                return Result.Fail($"Appointment with the id {request.Id} not found", "INVALID_ID");
            }

            if(appointmentDomain.Status != AppointmentStatus.Pending)
            {
                logger.LogWarning("Appointment with id {Id} cannot be updated, status is {Status}", request.Id, appointmentDomain.Status);
                return Result.Fail("Only pending appointments can have their status changed", "INVALID_STATUS");
            }

            appointmentDomain.Status = request.Status;

            await appointmentRepository.UpdateAsync(appointmentDomain);

            logger.LogInformation("Appointment with id {Id} status updated to {Status}", request.Id, request.Status);

            return Result.Ok("Appointment status updated");
        }
    }
}