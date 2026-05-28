using AutoMapper;
using HospitalManagement.Common;
using HospitalManagement.Models.Domain;
using HospitalManagement.Models.DTOs.DoctorSchedule;
using HospitalManagement.Repositories.Interfaces;
using HospitalManagement.Services.Interfaces;

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
            var existing = await doctorScheduleRepository.GetByDoctorIdAndDayAsync(request.DoctorId, request.DayOfWeek);

            if(existing != null)
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

        public Task<Result<DoctorScheduleResponseDto>> Delete(int id)
        {
            throw new NotImplementedException();
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

        public async Task<Result<DoctorScheduleResponseDto>> GetByIdAsync(int id)
        {
            var doctorScheduleDomain = await doctorScheduleRepository.GetByIdAsync(id);

            if(doctorScheduleDomain == null)
            {
                logger.LogWarning("Doctor schedule for id {Id} not found", id);
                return Result<DoctorScheduleResponseDto>.Fail($"Patient with id {id} not found", "INVALID_ID");
            }

            var result = mapper.Map<DoctorScheduleResponseDto>(doctorScheduleDomain);

            return Result<DoctorScheduleResponseDto>.Ok(result);
        }

        public Task<Result<DoctorScheduleUpdateResponseDto>> UpdateAsync(DoctorScheduleUpdateRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
