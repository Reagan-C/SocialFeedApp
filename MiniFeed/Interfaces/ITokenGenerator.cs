using MiniFeed.Models;

namespace MiniFeed.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
    }
}
