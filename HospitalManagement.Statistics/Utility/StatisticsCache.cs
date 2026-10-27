using HospitalManagement.Shared.Common;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace HospitalManagement.Statistics.Services.Utility
{
    public class StatisticsCache
    {
        private static readonly DistributedCacheEntryOptions Options =
            new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) };

        private readonly IDistributedCache cache;
        private readonly ILogger<StatisticsCache> logger;

        public StatisticsCache(IDistributedCache cache, ILogger<StatisticsCache> logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<Result<T>> GetOrSetAsync<T>(string name, DateOnly from, DateOnly to,
            Func<Task<Result<T>>> build)
        {
            var key = $"stats:{name}:{from:yyyy-MM-dd}:{to:yyyy-MM-dd}";

            try
            {
                var cached = await cache.GetStringAsync(key);
                if (cached != null)
                {
                    return Result<T>.Ok(JsonSerializer.Deserialize<T>(cached)!);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("Cache read failed for {Key}: {Message}", key, ex.Message);
            }

            var result = await build();

            if (result.Success)
            {
                try
                {
                    await cache.SetStringAsync(key, JsonSerializer.Serialize(result.Data), Options);
                }
                catch (Exception ex)
                {
                    logger.LogWarning("Cache write failed for {Key}: {Message}", key, ex.Message);
                }
            }

            return result;
        }
    }
}