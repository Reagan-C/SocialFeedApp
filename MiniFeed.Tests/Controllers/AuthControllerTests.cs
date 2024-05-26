
using Microsoft.AspNetCore.Mvc;
using MiniFeed.Controllers;
using MiniFeed.DTO;
using MiniFeed.Interfaces;
using Moq;
using Xunit;

namespace MiniFeed.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _authController.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _authController.Register(new UserRegistrationRequest());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenServiceReturnsError()
        {
            // Arrange
            var registrationRequest = new UserRegistrationRequest();
            _mockAuthService.Setup(s => s.Register(registrationRequest)).ReturnsAsync("Error");

            // Act
            var result = await _authController.Register(registrationRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registrationRequest = new UserRegistrationRequest();
            _mockAuthService.Setup(s => s.Register(registrationRequest)).ReturnsAsync(string.Empty);

            // Act
            var result = await _authController.Register(registrationRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _authController.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _authController.login(new LoginRequest());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenLoginFails()
        {
            // Arrange
            var loginRequest = new LoginRequest();
            _mockAuthService.Setup(s => s.Login(loginRequest)).ReturnsAsync(new LoginResponse { UserResponse = null });

            // Act
            var result = await _authController.login(loginRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginRequest = new LoginRequest();
            _mockAuthService.Setup(s => s.Login(loginRequest)).ReturnsAsync(new LoginResponse { UserResponse = new RetrievedUserResponse() });

            // Act
            var result = await _authController.login(loginRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AssignRole_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _authController.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _authController.assignRole(new AssignRoleRequest());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AssignRole_ReturnsBadRequest_WhenServiceReturnsError()
        {
            // Arrange
            var roleRequest = new AssignRoleRequest();
            _mockAuthService.Setup(s => s.AssignRole(roleRequest)).ReturnsAsync(false);

            // Act
            var result = await _authController.assignRole(roleRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AssignRole_ReturnsOk_WhenRoleAssignmentIsSuccessful()
        {
            // Arrange
            var roleRequest = new AssignRoleRequest { RoleName = "Admin" };
            _mockAuthService.Setup(s => s.AssignRole(roleRequest)).ReturnsAsync(true);

            // Act
            var result = await _authController.assignRole(roleRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}

