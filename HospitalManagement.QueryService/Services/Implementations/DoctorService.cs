using AutoMapper;
using HospitalManagement.QueryService.Clients.Implementations;
using HospitalManagement.QueryService.Clients.Interfaces;
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
        private readonly IAppointmentServiceClient appointmentServiceClient;
        private readonly IMapper mapper;
        private readonly ILogger<DoctorService> logger;
        private readonly IDistributedCache cache;
        private readonly IAsyncPolicy cachePolicy;

        public DoctorService(IDoctorRepository doctorRepository, IAppointmentServiceClient appointmentServiceClient,
            IMapper mapper, ILogger<DoctorService> logger,
            IDistributedCache cache, IAsyncPolicy cachePolicy)
        {
            this.doctorRepository = doctorRepository;
            this.appointmentServiceClient = appointmentServiceClient;
            this.mapper = mapper;
            this.logger = logger;
            this.cache = cache;
            this.cachePolicy = cachePolicy;
        }

        public async Task<Result<PagedResult<DoctorResponseDto>>> GetAllAsync(DoctorFilterDto filter)
        {
            var (doctors, totalCount) = await doctorRepository.GetAllAsync(filter);
            var mapped = mapper.Map<List<DoctorResponseDto>>(doctors);

            var pagedResult = new PagedResult<DoctorResponseDto>
            {
                Items = mapped,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return Result<PagedResult<DoctorResponseDto>>.Ok(pagedResult);
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
                logger.LogInformation("Doctor with id {id} cached", id);
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

        public async Task<Result<List<DoctorResponseDto>>> GetPopularDoctorsAsync(int count)
        {
            var doctorIds = await appointmentServiceClient.GetPopularDoctorIdsAsync(count);
            logger.LogInformation("GetPopularDoctorsAsync received {Count} ids: {Ids}", doctorIds?.Count, string.Join(",", doctorIds ?? new List<int>()));
            if (doctorIds == null)
            {
                logger.LogWarning("Could not retrieve popular doctor ids");
                return Result<List<DoctorResponseDto>>.Fail("Could not retrieve popular doctors", "HISTORY_UNAVAILABLE",
                    ErrorType.UpstreamFailure);
            }

            if(doctorIds.Count == 0)
            {
                return Result<List<DoctorResponseDto>>.Ok(new List<DoctorResponseDto>());
            }

            var doctors  = await doctorRepository.GetByIdsAsync(doctorIds);
            logger.LogInformation("GetByIdsAsync returned {Count} doctors", doctors.Count);

            var ordered = doctorIds
                .Select(id => doctors.FirstOrDefault(d => d.Id == id))
                .Where(d => d != null)
                .ToList();
            var mapped = mapper.Map<List<DoctorResponseDto>>(ordered);

            return Result<List<DoctorResponseDto>>.Ok(mapped);
        }
    }
}