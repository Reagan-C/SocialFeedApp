using Microsoft.EntityFrameworkCore;
using MiniFeed.Data;
using MiniFeed.Models;

namespace MiniFeed.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly AppDbContext _context;

        public LikeRepository(AppDbContext context)
        {
            _context=context;
        }

        public async Task AddLike(Like entity)
        {
            _context.Likes.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Like> GetLike(string userId, int postId)
        {
            var like = await _context.Likes
                        .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
            return like;
        }
    }
}
