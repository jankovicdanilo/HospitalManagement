using AutoMapper;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Models.DTOs.DoctorSchedule;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Events;
using MassTransit;

namespace HospitalManagement.CommandService.Services.Implementations
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IDoctorScheduleRepository doctorScheduleRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorScheduleService> logger;
        private readonly IPublishEndpoint publishEndpoint;

        public DoctorScheduleService(IDoctorScheduleRepository doctorScheduleRepository, IMapper mapper,
            ILogger<DoctorScheduleService> logger, IPublishEndpoint publishEndpoint)
        {
            this.doctorScheduleRepository = doctorScheduleRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.publishEndpoint = publishEndpoint;
        }

        public async Task<Result<DoctorScheduleCreateResponseDto>> CreateAsync(DoctorScheduleCreateRequestDto request)
        {
            var doctorExists = await doctorScheduleRepository.DoctorExists(request.DoctorId);
            if (!doctorExists)
            {
                logger.LogWarning("Doctor with id {DoctorId} not found", request.DoctorId);
                return Result<DoctorScheduleCreateResponseDto>.Fail($"Doctor with id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            }
            var existing = await doctorScheduleRepository.GetByDoctorIdAndDayAsync(request.DoctorId, request.DayOfWeek);
            if (existing != null)
            {
                logger.LogWarning("Doctor {DoctorId} already has a schedule for {DayOfWeek}", request.DoctorId, request.DayOfWeek);
                return Result<DoctorScheduleCreateResponseDto>.Fail($"Doctor already has a schedule for {request.DayOfWeek}", "DUPLICATE_SCHEDULE");
            }
            var doctorScheduleDomain = mapper.Map<DoctorSchedule>(request);

            logger.LogInformation("Doctor schedule created with id {Id}, DoctorScheduleCreated event published", doctorScheduleDomain.Id);

            doctorScheduleDomain = await doctorScheduleRepository.CreateAsync(doctorScheduleDomain);

            await publishEndpoint.Publish(new DoctorScheduleCreated(
                CorrelationId: Guid.NewGuid(),
                Id: doctorScheduleDomain!.Id,
                DoctorId: doctorScheduleDomain.DoctorId,
                DayOfWeek: doctorScheduleDomain.DayOfWeek,
                StartHour: doctorScheduleDomain.StartHour,
                EndHour: doctorScheduleDomain.EndHour));

            logger.LogInformation("Doctor schedule with id {Id} created", doctorScheduleDomain.Id);
            var result = mapper.Map<DoctorScheduleCreateResponseDto>(doctorScheduleDomain);
            return Result<DoctorScheduleCreateResponseDto>.Ok(result);
        }

        public async Task<Result<DoctorScheduleUpdateResponseDto>> UpdateAsync(DoctorScheduleUpdateRequestDto request)
        {
            var existing = await doctorScheduleRepository.GetByIdAsync(request.Id);
            if (existing == null)
            {
                logger.LogWarning("Doctor schedule with id {Id} not found", request.Id);
                return Result<DoctorScheduleUpdateResponseDto>.Fail($"Doctor schedule with id {request.Id} not found", "INVALID_ID");
            }
            var duplicate = await doctorScheduleRepository.GetByDoctorIdAndDayAsync(existing.DoctorId, request.DayOfWeek);
            if (duplicate != null && duplicate.Id != request.Id)
            {
                logger.LogWarning("Doctor {DoctorId} already has a schedule for {DayOfWeek}", existing.DoctorId, request.DayOfWeek);
                return Result<DoctorScheduleUpdateResponseDto>.Fail($"Doctor already has a schedule for {request.DayOfWeek}", "DUPLICATE_SCHEDULE");
            }
            existing.DayOfWeek = request.DayOfWeek;
            existing.StartHour = request.StartHour;
            existing.EndHour = request.EndHour;
            existing = await doctorScheduleRepository.UpdateAsync(existing);


            await publishEndpoint.Publish(new DoctorScheduleUpdated(
                CorrelationId: Guid.NewGuid(),
                Id: existing.Id,
                DoctorId: existing.DoctorId,
                DayOfWeek: existing.DayOfWeek,
                StartHour: existing.StartHour,
                EndHour: existing.EndHour));

            logger.LogInformation("Doctor schedule with id {Id} updated, DoctorScheduleUpdated event published", existing.Id);
            var result = mapper.Map<DoctorScheduleUpdateResponseDto>(existing);
            return Result<DoctorScheduleUpdateResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var doctorScheduleDomain = await doctorScheduleRepository.Delete(id);
            if (doctorScheduleDomain == null)
            {
                logger.LogWarning("Doctor schedule for id {Id} not found for deletion", id);
                return Result.Fail($"Doctor schedule id {id} not found", "INVALID_ID");
            }

            await publishEndpoint.Publish(new DoctorScheduleDeleted(
                CorrelationId: Guid.NewGuid(),
                Id: id));

            logger.LogInformation("Doctor schedule with id {Id} deleted, DoctorScheduleDeleted event published", id);
            return Result.Ok($"Doctor schedule with id {id} deleted");
        }
    }
}