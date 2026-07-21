using AutoMapper;
using HospitalManagement.Shared.Models.DTOs.DoctorSchedule;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.QueryService.Services.Implementations
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

        public async Task<Result<DoctorScheduleResponseDto>> GetByIdAsync(int id)
        {
            var schedule = await doctorScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                logger.LogWarning("Doctor schedule for id {Id} not found", id);
                return Result<DoctorScheduleResponseDto>.Fail($"Doctor schedule with id {id} not found", "INVALID_ID",
                    ErrorType.NotFound);
            }
            var result = mapper.Map<DoctorScheduleResponseDto>(schedule);
            return Result<DoctorScheduleResponseDto>.Ok(result);
        }

        public async Task<Result<List<DoctorScheduleResponseDto>>> GetAllByDoctorIdAsync(int doctorId)
        {
            bool exists = await doctorScheduleRepository.DoctorExists(doctorId);
            if (!exists)
            {
                logger.LogWarning("Doctor with id {DoctorId} not found", doctorId);
                return Result<List<DoctorScheduleResponseDto>>.Fail($"Doctor with id {doctorId} not found", 
                    "INVALID_DOCTOR_ID", ErrorType.NotFound);
            }
            var schedules = await doctorScheduleRepository.GetAllByDoctorIdAsync(doctorId);
            var result = mapper.Map<List<DoctorScheduleResponseDto>>(schedules);
            return Result<List<DoctorScheduleResponseDto>>.Ok(result);
        }

        public async Task<Result<DoctorScheduleResponseDto>> GetByDoctorIdAndDayAsync(int doctorId, DayOfWeek dayOfWeek)
        {
            bool exists = await doctorScheduleRepository.DoctorExists(doctorId);
            if (!exists)
            {
                logger.LogWarning("Doctor with id {DoctorId} not found", doctorId);
                return Result<DoctorScheduleResponseDto>.Fail($"Doctor with id {doctorId} not found", "INVALID_DOCTOR_ID", 
                    ErrorType.NotFound);
            }
            var schedule = await doctorScheduleRepository.GetByDoctorIdAndDayAsync(doctorId, dayOfWeek);
            if (schedule == null)
            {
                logger.LogWarning("Doctor with id {Id} doesn't work on {Day}", doctorId, dayOfWeek);
                return Result<DoctorScheduleResponseDto>.Fail($"Doctor does not work on {dayOfWeek}", "DOCTOR_NOT_AVAILABLE",
                    ErrorType.Conflict);
            }
            var result = mapper.Map<DoctorScheduleResponseDto>(schedule);
            return Result<DoctorScheduleResponseDto>.Ok(result);
        }
    }
}