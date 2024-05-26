using MiniFeed.Models;

namespace MiniFeed.Repositories
{
    public interface ILikeRepository
    {
        Task<Like> GetLike(string userId, int postId);
        Task AddLike(Like entity);
    }
}
