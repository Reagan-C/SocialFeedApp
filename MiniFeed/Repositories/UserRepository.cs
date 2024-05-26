using Microsoft.EntityFrameworkCore;
using MiniFeed.Data;
using MiniFeed.Models;

namespace MiniFeed.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext=dbContext;
        }

        public ICollection<User> GetAllUsers()
        {
            return _dbContext.Users.OrderBy(u => u.Id).ToList();
        }

        public  User GetUserByEmail(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            return user;
        }

        public async Task<User> GetUserById(string Id)
        {
            var user = await _dbContext.Users.FindAsync(Id);
            return user;
        }

        public async Task<User> GetUserAsync(string userName)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
            return user;
        }

        public async Task SaveChanges()
        {
           await _dbContext.SaveChangesAsync();
        }

    }
}
