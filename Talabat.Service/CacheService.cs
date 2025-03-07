using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;

namespace Talabat.Service
{
    public class CacheService(ICacheRepository cacheRepository) : ICacheService
    {
        public async Task<string?> GetAsync(string key)
            => await cacheRepository.GetAsync(key);

        public async Task SetAsync(string key, object value, TimeSpan duration) 
            => await cacheRepository.SetAsync(key, value, duration);
    }
}
