using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MiniFeed.Controllers;
using MiniFeed.Interfaces;
using MiniFeed.Models;
using MiniFeed.Repositories;
using Moq;
using System.Security.Claims;
using Xunit;

namespace MiniFeed.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IPostService> _postServiceMock;
        private readonly Mock<IFollowRepository> _followRepoMock;
        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly Mock<ILikeRepository> _likeRepoMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _postServiceMock = new Mock<IPostService>();
            _followRepoMock = new Mock<IFollowRepository>();
            _cacheMock = new Mock<IMemoryCache>();
            _likeRepoMock = new Mock<ILikeRepository>();

            _controller = new UserController(_userRepoMock.Object, _postServiceMock.Object,
                _followRepoMock.Object, _cacheMock.Object, _likeRepoMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task FollowUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var username = "nonexistentuser";
            _userRepoMock.Setup(r => r.GetUserAsync(username))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.FollowUser(username);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task FollowUser_SelfFollow_ReturnsBadRequest()
        {
            // Arrange
            var userName = "test-user";
            var userId = "test-user-id";
            var currentUser = new User { Id = userId, UserName = userName };

            _userRepoMock.Setup(r => r.GetUserById(userId))
                .ReturnsAsync(currentUser);
            _userRepoMock.Setup(repo => repo.GetUserAsync(userName))
                .ReturnsAsync(currentUser);

            // Act
            var result = await _controller.FollowUser(userName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("You cannot follow yourself.", badRequestResult.Value);
        }

        [Fact]
        public async Task FollowUser_AlreadyFollowing_ReturnsBadRequest()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var userToFollowId = "usertofollow";
            var currentUser = new User { Id = currentUserId, UserName = "currentUser" };
            var userToFollow = new User { Id = userToFollowId, UserName = "userToFollowUsername" };
            var existingFollow = new Follow { FollowerId = currentUserId, FollowedId = userToFollowId };
            _userRepoMock.Setup(r => r.GetUserById(currentUserId))
                .ReturnsAsync(currentUser);
            _userRepoMock.Setup(r => r.GetUserAsync(userToFollow.UserName))
                .ReturnsAsync(userToFollow);
            _followRepoMock.Setup(r => r.GetFollow(currentUserId, userToFollowId))
                .ReturnsAsync(existingFollow);
            
            // Act
            var result = await _controller.FollowUser(userToFollow.UserName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Already following the user", badRequestResult.Value);
        }

        [Fact]
        public async Task FollowUser_Success_ReturnsOk()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var userToFollowId = "usertofollow";
            var currentUser = new User { Id = currentUserId, UserName = "currentU" };
            var userToFollow = new User { Id = userToFollowId, UserName = "UserToFollow" };
            _userRepoMock.Setup(r => r.GetUserById(currentUserId))
                .ReturnsAsync(currentUser);
            _userRepoMock.Setup(r => r.GetUserAsync(userToFollow.UserName))
                .ReturnsAsync(userToFollow);
            _followRepoMock.Setup(r => r.GetFollow(currentUserId, userToFollowId))
                .ReturnsAsync((Follow)null);
           
            // Act
            var result = await _controller.FollowUser(userToFollow.UserName);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _followRepoMock.Verify(r => r.AddFollow(It.IsAny<Follow>()), Times.Once);
            _cacheMock.Verify(c => c.Remove(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UnfollowUser_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var username = "nonexistentuser";
            _userRepoMock.Setup(r => r.GetUserAsync(username))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.UnfollowUser(username);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UnfollowUser_SelfUnfollow_ReturnsBadRequest()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var currentUser = new User { Id = currentUserId, UserName = "currentuser" };
            _userRepoMock.Setup(r => r.GetUserById(currentUserId))
                .ReturnsAsync(currentUser);
            _userRepoMock.Setup(r => r.GetUserAsync(currentUser.UserName))
                .ReturnsAsync(currentUser);
           
            // Act
            var result = await _controller.UnfollowUser(currentUser.UserName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UnfollowUser_NotFollowing_ReturnsBadRequest()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var userToUnfollowId = "usertounfollow";
            var currentUser = new User { Id = currentUserId };
            var userToUnfollow = new User { Id = userToUnfollowId };
            _userRepoMock.Setup(r => r.GetUserById(currentUserId))
                .ReturnsAsync(currentUser);
            _userRepoMock.Setup(r => r.GetUserAsync(userToUnfollow.UserName))
                .ReturnsAsync(userToUnfollow);
            _followRepoMock.Setup(r => r.GetFollow(currentUserId, userToUnfollowId))
                .ReturnsAsync((Follow)null);
            
            // Act
            var result = await _controller.UnfollowUser(userToUnfollow.UserName);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UnfollowUser_Success_ReturnsOk()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var userToUnfollowId = "usertounfollow";
            var currentUser = new User { Id = currentUserId, UserName = "currentU" };
            var userToUnfollow = new User { Id = userToUnfollowId, UserName = "UserToFollow" };
            var existingFollow = new Follow { FollowerId = currentUserId, FollowedId = userToUnfollowId };
            _userRepoMock.Setup(r => r.GetUserById(currentUserId))
                .ReturnsAsync(currentUser);
            _userRepoMock.Setup(r => r.GetUserAsync(userToUnfollow.UserName))
                .ReturnsAsync(userToUnfollow);
            _followRepoMock.Setup(r => r.GetFollow(currentUserId, userToUnfollowId))
                .ReturnsAsync(existingFollow);
            
            // Act
            var result = await _controller.UnfollowUser(userToUnfollow.UserName);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _followRepoMock.Verify(r => r.UnFollow(existingFollow), Times.Once);
        }

        [Fact]
        public async Task LikePost_PostNotFound_ReturnsNotFound()
        {
            // Arrange
            var postId = 1;
            _postServiceMock.Setup(s => s.FindAsync(postId))
                .ReturnsAsync((Post)null);

            // Act
            var result = await _controller.LikePost(postId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task LikePost_AlreadyLiked_ReturnsBadRequest()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var postId = 1;
            var post = new Post { Id = postId };
            var existingLike = new Like { UserId = currentUserId, PostId = postId };
            _postServiceMock.Setup(s => s.FindAsync(postId))
                .ReturnsAsync(post);
            _likeRepoMock.Setup(r => r.GetLike(currentUserId, postId))
                .ReturnsAsync(existingLike);
           
            // Act
            var result = await _controller.LikePost(postId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task LikePost_Success_ReturnsOk()
        {
            // Arrange
            var currentUserId = "test-user-id";
            var postId = 1;
            var post = new Post { Id = postId };
            _postServiceMock.Setup(s => s.FindAsync(postId))
                .ReturnsAsync(post);
            _likeRepoMock.Setup(r => r.GetLike(currentUserId, postId))
                .ReturnsAsync((Like)null);
           
            // Act
            var result = await _controller.LikePost(postId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, post.LikesCount);
            _likeRepoMock.Verify(r => r.AddLike(It.IsAny<Like>()), Times.Once);
        }
    }
}
