using Microsoft.Extensions.Caching.Memory;
using MiniFeed.DTO;
using MiniFeed.Interfaces;

namespace MiniFeed.Services
{
    public class MemoryCacheWrapper : IMemoryCacheWrapper
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheWrapper(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool TryGetValue(string key, out IEnumerable<GetPostResponse> value)
        {
            return _cache.TryGetValue(key, out value);
        }

        public void Set(string key, IEnumerable<GetPostResponse> value, MemoryCacheEntryOptions options)
        {
            _cache.Set(key, value, options);
        }
    }
}
