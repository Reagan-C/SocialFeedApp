using MiniFeed.Models;

namespace MiniFeed.Repositories
{
    public interface IFollowRepository
    {
        Task<Follow> GetFollow (string followerId, string followedId);
        Task AddFollow(Follow entity);
        Task UnFollow(Follow entity);
    }
}
