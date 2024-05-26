
using Microsoft.EntityFrameworkCore;
using MiniFeed.Data;
using MiniFeed.Models;

namespace MiniFeed.Repositories
{
    public class FollowRepository : IFollowRepository
    {
        private readonly AppDbContext _context;

        public FollowRepository(AppDbContext context)
        {
            _context=context;
        }

        public async Task AddFollow(Follow entity)
        {
            _context.Follows.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Follow> GetFollow(string followerId, string followedId)
        {
            var follow = await _context.Follows
                        .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
            return follow;
        }

        public async Task UnFollow(Follow entity)
        {
            _context.Follows.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
