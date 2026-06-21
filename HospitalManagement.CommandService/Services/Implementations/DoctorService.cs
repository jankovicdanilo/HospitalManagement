using AutoMapper;
using HospitalManagement.CommandService.Models.Doctor;
using HospitalManagement.CommandService.Models.Domain;
using HospitalManagement.CommandService.Models.DTOs.Doctor;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Common;

namespace HospitalManagement.CommandService.Services.Implementations
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

        public async Task<Result<DoctorResponseDto>> CreateAsync(DoctorCreateRequestDto request)
        {
            var doctorDomain = mapper.Map<Doctor>(request);
            doctorDomain = await doctorRepository.CreateAsync(doctorDomain);
            logger.LogInformation("Doctor created with id {Id}", doctorDomain.Id);
            var result = mapper.Map<DoctorResponseDto>(doctorDomain);
            return Result<DoctorResponseDto>.Ok(result);
        }

        public async Task<Result<DoctorResponseDto>> UpdateAsync(DoctorUpdateRequestDto request)
        {
            var doctorDomain = await doctorRepository.GetByIdAsync(request.Id);
            if (doctorDomain is null)
            {
                logger.LogWarning("Doctor with id {Id} not found for update", request.Id);
                return Result<DoctorResponseDto>.Fail($"Doctor with the id {request.Id} doesn't exist!", "INVALID_ID", ErrorType.NotFound);
            }
            mapper.Map(request, doctorDomain);
            doctorDomain = await doctorRepository.UpdateAsync(doctorDomain);
            logger.LogInformation("Doctor with id {Id} updated", doctorDomain.Id);
            var result = mapper.Map<DoctorResponseDto>(doctorDomain);
            return Result<DoctorResponseDto>.Ok(result);
        }

        public async Task<Result> Delete(int id)
        {
            var doctor = await doctorRepository.GetByIdAsync(id);
            if (doctor is null)
            {
                logger.LogWarning("Doctor with id {Id} not found for deletion", id);
                return Result.Fail($"Doctor with the id {id} does not exist", "INVALID_ID", ErrorType.NotFound);
            }
            await doctorRepository.Delete(id);
            logger.LogInformation("Doctor with id {Id} deleted", id);
            return Result.Ok("Doctor has been deleted!");
        }
    }
}