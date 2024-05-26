using MiniFeed.DTO;

namespace MiniFeed.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(UserRegistrationRequest requestDto);
        Task<LoginResponse> Login(LoginRequest loginRequest);
        Task<bool> AssignRole(AssignRoleRequest assignRoleRequest);
    }
}
