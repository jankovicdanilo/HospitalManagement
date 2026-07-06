using AutoMapper;
using HospitalManagement.QueryService.Repositories.Interfaces;
using HospitalManagement.QueryService.Services.Interfaces;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Models.DTOs.Doctor;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Caching;
using Polly.CircuitBreaker;
using System.Text.Json;

namespace HospitalManagement.QueryService.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository doctorRepository;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorService> logger;
        private readonly IDistributedCache cache;
        private readonly IAsyncPolicy cachePolicy;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper, ILogger<DoctorService> logger,
            IDistributedCache cache, IAsyncPolicy cachePolicy)
        {
            this.doctorRepository = doctorRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.cache = cache;
            this.cachePolicy = cachePolicy;
        }

        public async Task<Result<List<DoctorResponseDto>>> GetAllAsync()
        {
            var doctors = await doctorRepository.GetAllAsync();
            var result = mapper.Map<List<DoctorResponseDto>>(doctors);
            return Result<List<DoctorResponseDto>>.Ok(result);
        }

        public async Task<Result<DoctorResponseDto>> GetByIdAsync(int id)
        {
            string cacheKey = $"doctor:{id}";
            string? cached = null;
            try
            {
                cached = await cachePolicy.ExecuteAsync(() => cache.GetStringAsync(cacheKey));
            }
            catch (BrokenCircuitException)
            {
                logger.LogDebug("Redus circuit open, skipping cache read for {Key}", cacheKey);
            }
            catch(Exception ex)
            {
                logger.LogWarning("Cache unavailable for key {Key}, falling back to DB: {Message}", cacheKey, ex.Message);
            }

            if(cached != null)
            {
                logger.LogInformation("Cache hit for doctor {Id}", id);
                var cachedDto = JsonSerializer.Deserialize<DoctorResponseDto>(cached);
                return Result<DoctorResponseDto>.Ok(cachedDto!);
            }

            var doctor = await doctorRepository.GetByIdAsync(id);
            if (doctor is null)
            {
                logger.LogWarning("Doctor with id {Id} not found", id);
                return Result<DoctorResponseDto>.Fail($"Doctor with the id {id} was not found", "INVALID_ID", ErrorType.NotFound);
            }

            var result = mapper.Map<DoctorResponseDto>(doctor);

            try
            {
                await cachePolicy.ExecuteAsync(() => cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }));
            }
            catch (BrokenCircuitException)
            {
                logger.LogDebug("Redis circuit open, skipping cache write for {Key}", cacheKey);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Failed to write cache for key {Key}: {Message}", cacheKey, ex.Message);
            }

            return Result<DoctorResponseDto>.Ok(result);
        }
    }
}