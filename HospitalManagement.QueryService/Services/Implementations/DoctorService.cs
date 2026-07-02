using AutoMapper;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Doctor;

namespace HospitalManagement.QueryService.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorService> logger;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper, ILogger<DoctorService> logger)
        {
            this.doctorRepository = doctorRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<Result<List<DoctorResponseDto>>> GetAllAsync()
        {
            var doctors = await doctorRepository.GetAllAsync();
            var result = mapper.Map<List<DoctorResponseDto>>(doctors);
            return Result<List<DoctorResponseDto>>.Ok(result);
        }

        public async Task<Result<DoctorResponseDto>> GetByIdAsync(int id)
        {
            var doctor = await doctorRepository.GetByIdAsync(id);
            if (doctor is null)
            {
                logger.LogWarning("Doctor with id {Id} not found", id);
                return Result<DoctorResponseDto>.Fail($"Doctor with the id {id} was not found", "INVALID_ID", ErrorType.NotFound);
            }
            var result = mapper.Map<DoctorResponseDto>(doctor);
            return Result<DoctorResponseDto>.Ok(result);
        }
    }
}