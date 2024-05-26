using MiniFeed.Models;

namespace MiniFeed.Repositories
{
    public interface IUserRepository
    {
        User GetUserByEmail(string email);
        Task<User> GetUserAsync(string userName);
        Task<User> GetUserById(string Id);
        ICollection<User> GetAllUsers();
        Task SaveChanges();
    }
}
