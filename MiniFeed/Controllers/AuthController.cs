using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniFeed.DTO;
using MiniFeed.Interfaces;

namespace MiniFeed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService=authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest registrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.Register(registrationRequest);

            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(result);
            }

            return Ok("account creation successful");
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> login(LoginRequest requestDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var logInResponse = await _authService.Login(requestDto);
            if (logInResponse.UserResponse == null)
            {
                return BadRequest("Username or password incorrect");
            }
            return Ok(logInResponse);
        }

        [HttpPost]
        [Route("assignRole")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> assignRole([FromBody] AssignRoleRequest roleDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var assignRoleSuccess = await _authService.AssignRole(roleDto);

            if (!assignRoleSuccess)
            {
                return BadRequest("User email / role name incorrect");
            }

            return Ok(roleDto.RoleName + " role assigned");
        }
    }

}
