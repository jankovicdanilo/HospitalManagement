using AutoMapper;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.DoctorSchedule;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs;

namespace HospitalManagement.Services.Implementations
{
    public class DoctorScheduleService : IDoctorScheduleService
    {
        private readonly IDoctorScheduleRepository doctorScheduleRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorScheduleService> logger;

        public DoctorScheduleService(IDoctorScheduleRepository doctorScheduleRepository, IMapper mapper,
            ILogger<DoctorScheduleService> logger)
        {
            this.doctorScheduleRepository = doctorScheduleRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Result<DoctorScheduleCreateResponseDto>> CreateAsync(DoctorScheduleCreateRequestDto request)
        {
            var doctorExists = await doctorScheduleRepository.DoctorExists(request.DoctorId);
            var existing = await doctorScheduleRepository.GetByDoctorIdAndDayAsync(request.DoctorId, request.DayOfWeek);

            if (!doctorExists)
            {
                logger.LogWarning("Doctor with id {DoctorId} not found", request.DoctorId);
                return Result<DoctorScheduleCreateResponseDto>.Fail($"Doctor with id {request.DoctorId} not found", "INVALID_DOCTOR_ID");
            }

            if (existing != null)
            {
                logger.LogWarning("Doctor {DoctorId} already has a schedule for {DayOfWeek}", request.DoctorId, request.DayOfWeek);
                return Result<DoctorScheduleCreateResponseDto>.Fail($"Doctor already has a schedule for {request.DayOfWeek}", "DUPLICATE_SCHEDULE");
            }

            var doctorScheduleDomain = mapper.Map<DoctorSchedule>(request);

            doctorScheduleDomain = await doctorScheduleRepository.CreateAsync(doctorScheduleDomain);

            logger.LogInformation("Doctor schedule with id {Id} created", doctorScheduleDomain.Id);

            var result = mapper.Map<DoctorScheduleCreateResponseDto>(doctorScheduleDomain);

            return Result<DoctorScheduleCreateResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var doctorScheduleDomain = await doctorScheduleRepository.Delete(id);

            if(doctorScheduleDomain == null)
            {
                logger.LogWarning("Doctor schedule for id {Id} not found for deletion", id);
                return Result.Fail($"Doctor schedule id {id} not found", "INVALID_ID");
            }

            logger.LogInformation("Doctor schedule with id {Id} deleted", id);

            return Result.Ok($"Doctor schedule with id {id} deleted");
        }

        public async Task<Result<List<DoctorScheduleResponseDto>>> GetAllByDoctorIdAsync(int doctorId)
        {
            bool exists = await doctorScheduleRepository.DoctorExists(doctorId);

            if (!exists)
            {
                logger.LogWarning("Doctor with id {DoctorId} not found", doctorId);
                return Result<List<DoctorScheduleResponseDto>>.Fail($"Doctor with id {doctorId} not found", "INVALID_DOCTOR_ID");
            }

            var doctorScheduleDomainList = await doctorScheduleRepository.GetAllByDoctorIdAsync(doctorId);

            var result = mapper.Map<List<DoctorScheduleResponseDto>>(doctorScheduleDomainList);

            return Result<List<DoctorScheduleResponseDto>>.Ok(result);
        }

        public async Task<Result<DoctorScheduleResponseDto>> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            bool exists = await doctorScheduleRepository.DoctorExists(doctorId);

            if (!exists)
            {
                logger.LogWarning("Doctor with id {DoctorId} not found", doctorId);
                return Result<DoctorScheduleResponseDto>.Fail($"Doctor with id {doctorId} not found", "INVALID_DOCTOR_ID");
            }

            var doctorScheduleDomain = await doctorScheduleRepository.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek);

            if(doctorScheduleDomain == null)
            {
                logger.LogWarning("Doctor with id {Id} doesn't work on this {Day}", doctorId, dayOfWeek);
                return Result<DoctorScheduleResponseDto>.Fail($"Doctor does not work on {dayOfWeek}", "DOCTOR_NOT_AVAILABLE");
            }

            var result = mapper.Map<DoctorScheduleResponseDto>(doctorScheduleDomain);

            return Result<DoctorScheduleResponseDto>.Ok(result);
        }

        public async Task<Result<DoctorScheduleResponseDto>> GetByIdAsync(int id)
        {
            var doctorScheduleDomain = await doctorScheduleRepository.GetByIdAsync(id);

            if(doctorScheduleDomain == null)
            {
                logger.LogWarning("Doctor schedule for id {Id} not found", id);
                return Result<DoctorScheduleResponseDto>.Fail($"Doctor schedule with id {id} not found", "INVALID_ID");
            }

            var result = mapper.Map<DoctorScheduleResponseDto>(doctorScheduleDomain);

            return Result<DoctorScheduleResponseDto>.Ok(result);
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

            logger.LogInformation("Doctor schedule with id {Id} updated", request.Id);

            var result = mapper.Map<DoctorScheduleUpdateResponseDto>(existing);

            return Result<DoctorScheduleUpdateResponseDto>.Ok(result);
        }
    }
}
