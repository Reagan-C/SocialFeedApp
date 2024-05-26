using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MiniFeed.DTO;
using MiniFeed.Interfaces;
using MiniFeed.Models;
using MiniFeed.Repositories;

namespace MiniFeed.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepo;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, 
            IUserRepository userRepo, ITokenGenerator tokenGenerator, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepo = userRepo;
            _tokenGenerator = tokenGenerator;
            _mapper=mapper;
        }

        public async Task<bool> AssignRole(AssignRoleRequest roleRequest)
        {
            var user = _userRepo.GetUserByEmail(roleRequest.Email);

            if (user == null || !_roleManager.RoleExistsAsync(roleRequest.RoleName).GetAwaiter().GetResult())
            {
                return false;
            }

            await _userManager.AddToRoleAsync(user, roleRequest.RoleName);
            return true;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            var user = _userRepo.GetUserByEmail(loginRequest.Email);

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (user == null || isValid == false)
            {
                return new LoginResponse() { Token = "", UserResponse = null };
            }

            var retrievedUser = _mapper.Map<RetrievedUserResponse>(user);

            LoginResponse loginResponse = new LoginResponse()
            {
                UserResponse = retrievedUser,
                Token = _tokenGenerator.GenerateToken(user)
            };
            return loginResponse;
        }

        public async Task<string> Register(UserRegistrationRequest requestDto)
        {
            var user = _mapper.Map<User>(requestDto);
            user.UserName = requestDto.Email;
            
            try
            {
                var result = await _userManager.CreateAsync(user, requestDto.Password);
                if (result.Succeeded)
                {
                    var roleAssign = await _userManager.AddToRoleAsync(user, "User");
                    if (!roleAssign.Succeeded)
                    {
                        return roleAssign.Errors.FirstOrDefault().Description;
                    }

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            return "Error encountered";
        }
    }
}
