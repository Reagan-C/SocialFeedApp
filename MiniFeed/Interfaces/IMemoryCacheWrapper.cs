using Microsoft.Extensions.Caching.Memory;
using MiniFeed.DTO;

namespace MiniFeed.Interfaces
{
    public interface IMemoryCacheWrapper
    {
        bool TryGetValue(string key, out IEnumerable<GetPostResponse> value);
        void Set(string key, IEnumerable<GetPostResponse> value, MemoryCacheEntryOptions options);
    }
}
