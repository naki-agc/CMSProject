using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CMSProject.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(15);

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var data = await _cache.GetAsync(key);
            if (data == null)
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime ?? _defaultExpiration,
                SlidingExpiration = TimeSpan.FromMinutes(15) // 15 dakika içinde erişilmezse cache'den silinir
            };

            var serializedData = JsonSerializer.SerializeToUtf8Bytes(value);
            await _cache.SetAsync(key, serializedData, options);

            // Cache yenileme için background job planlama
            await ScheduleCacheRefresh(key, value, options.AbsoluteExpirationRelativeToNow.Value);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expirationTime = null)
        {
            var result = await GetAsync<T>(key);
            if (result != null)
                return result;

            result = await factory();
            await SetAsync(key, result, expirationTime);
            return result;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await GetAsync<object>(key) != null;
        }

        private async Task ScheduleCacheRefresh<T>(string key, T value, TimeSpan expirationTime)
        {
            // Background job'u planla (Hangfire veya benzeri bir tool kullanılabilir)
            var refreshTime = expirationTime.Subtract(TimeSpan.FromMinutes(1)); // 1 dakika önce yenileme başlat

            // Örnek Hangfire kullanımı:
            // BackgroundJob.Schedule(() => RefreshCache(key, value), refreshTime);
        }

        private async Task RefreshCache<T>(string key, T value)
        {
            try
            {
                // Veriyi yeniden çek ve cache'i güncelle
                await SetAsync(key, value, _defaultExpiration);
                _logger.LogInformation($"Cache refreshed for key: {key}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error refreshing cache for key: {key}");
            }
        }
    }
}
