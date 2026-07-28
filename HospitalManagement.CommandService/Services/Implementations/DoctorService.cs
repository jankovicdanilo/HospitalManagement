using AutoMapper;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using HospitalManagement.Shared.Models.Domain;
using HospitalManagement.CommandService.Repositories.Interfaces;
using HospitalManagement.CommandService.Services.Interfaces;
using HospitalManagement.Shared.Common;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.CircuitBreaker;

namespace HospitalManagement.CommandService.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorService> logger;
        private readonly IDistributedCache cache;
        private readonly IAsyncPolicy cachePolicy;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper,
            ILogger<DoctorService> logger, IDistributedCache cache,
            IAsyncPolicy cachePolicy)
        {
            this.doctorRepository = doctorRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.cache = cache;
            this.cachePolicy = cachePolicy;
        }

        public async Task<Result<DoctorResponseDto>> CreateAsync(DoctorCreateRequestDto request)
        {
            var doctorExists = await doctorRepository.GetByEmailAsync(request.Email);
            if (doctorExists != null)
            {
                logger.LogWarning("Doctor creation failed, email {Email} already exists", request.Email);
                return Result<DoctorResponseDto>.Fail($"Email {request.Email} already exists", "INVALID_EMAIL",
                    ErrorType.Conflict);
            }

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
                return Result<DoctorResponseDto>.Fail($"Doctor with the id {request.Id} doesn't exist!", "INVALID_ID", 
                    ErrorType.NotFound);
            }

            var emailOwner = await doctorRepository.GetByEmailAsync(request.Email);
            if (emailOwner != null && emailOwner.Id != request.Id)
            {
                logger.LogWarning("Doctor update failed, email {Email} already exists", request.Email);
                return Result<DoctorResponseDto>.Fail($"Email {request.Email} already exists", "INVALID_EMAIL",
                    ErrorType.Conflict);
            }

            mapper.Map(request, doctorDomain);
            doctorDomain = await doctorRepository.UpdateAsync(doctorDomain);

            await InvalidateCacheAsync(doctorDomain!.Id);
            logger.LogInformation("Doctor updated with id {Id}, cache invalidated", doctorDomain.Id);

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

            await InvalidateCacheAsync(id);
            logger.LogInformation("Doctor deleted with id {Id}, cache invalidated", id);

            return Result.Ok("Doctor has been deleted!");
        }

        private async Task InvalidateCacheAsync(int id)
        {
            string cacheKey = $"doctor:{id}";
            try
            {
                await cachePolicy.ExecuteAsync(() => cache.RemoveAsync(cacheKey));
                logger.LogInformation("Cache invalidated for key {Key}", cacheKey);
            }
            catch (BrokenCircuitException)
            {
                logger.LogDebug("Redit circuit open, skipping cache invalidation for {Key}", cacheKey);
            }
            catch(Exception ex)
            {
                logger.LogWarning("Failed to invalidate cache for key {Key}: {Message}", cacheKey, ex.Message);
            }
        }
    }
}