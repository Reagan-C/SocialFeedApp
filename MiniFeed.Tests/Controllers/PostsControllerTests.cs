using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MiniFeed.Controllers;
using MiniFeed.DTO;
using MiniFeed.Interfaces;
using MiniFeed.Models;
using Moq;
using System.Security.Claims;
using Xunit;

namespace MiniFeed.Tests.Controllers
{
    public class PostsControllerTests
    {
        private readonly Mock<IPostService> _mockPostService;
        private readonly Mock<IMemoryCacheWrapper> _mockCache;
        private readonly PostsController _postsController;

        public PostsControllerTests()
        {
            _mockPostService = new Mock<IPostService>();
            _mockCache = new Mock<IMemoryCacheWrapper>();
            _postsController = new PostsController(_mockPostService.Object, _mockCache.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));

            _postsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task CreatePost_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _postsController.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _postsController.CreatePost(new CreatePostRequest());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreatePost_ReturnsCreated_WhenPostIsCreated()
        {
            // Arrange
            var createPostRequest = new CreatePostRequest();
            var post = new GetPostResponse { Id = 1 };
            _mockPostService.Setup(s => s.CreatePost ("test-user-id", createPostRequest)).Returns(Task.FromResult(post));

            // Act
            var result = await _postsController.CreatePost(createPostRequest);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal(nameof(_postsController.GetPost), createdAtRouteResult.RouteName);
            Assert.Equal(post.Id, createdAtRouteResult.RouteValues["id"]);
        }

        [Fact]
        public async Task GetPost_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _postsController.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _postsController.GetPost(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetPost_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            _mockPostService.Setup(s => s.GetPostById(1)).ReturnsAsync((GetPostResponse)null);

            // Act
            var result = await _postsController.GetPost(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetPost_ReturnsOk_WhenPostExists()
        {
            // Arrange
            var post = new GetPostResponse { Id = 1 };
            _mockPostService.Setup(s => s.GetPostById(1)).ReturnsAsync(post);

            // Act
            var result = await _postsController.GetPost(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(post, okResult.Value);
        }

        [Fact]
        public async Task DeletePost_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _postsController.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _postsController.DeletePost(1);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeletePost_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            var post = new GetPostResponse { Id = 1 };
            _mockPostService.Setup(s => s.GetPostById(1)).ReturnsAsync(post);

            // Act
            var result = await _postsController.DeletePost(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeletePost_ReturnsForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var post = new Post { Id = 1, UserId = "different-user-id", Text = "Testing" };
            _mockPostService.Setup(s => s.FindAsync(1)).ReturnsAsync(post);

            // Act
            var result = await _postsController.DeletePost(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task DeletePost_ReturnsNoContent_WhenPostIsDeleted()
        {
            // Arrange
            var post = new Post { Id = 1, UserId = "test-user-id" };
            _mockPostService.Setup(s => s.FindAsync(1)).ReturnsAsync(post);

            // Act
            var result = await _postsController.DeletePost(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePost_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _postsController.ModelState.AddModelError("Error", "Invalid model state");

            // Act
            var result = await _postsController.UpdatePost(1, new CreatePostRequest());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdatePost_ReturnsNotFound_WhenPostDoesNotExist()
        {
            // Arrange
            _mockPostService.Setup(s => s.GetPostById(1)).ReturnsAsync((GetPostResponse)null);

            // Act
            var result = await _postsController.UpdatePost(1, new CreatePostRequest());

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdatePost_ReturnsForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var post = new Post { Id = 1, UserId = "different-user-id" };
            _mockPostService.Setup(s => s.FindAsync(1)).ReturnsAsync(post);

            // Act
            var result = await _postsController.UpdatePost(1, new CreatePostRequest());

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdatePost_ReturnsOk_WhenPostIsUpdated()
        {
            // Arrange
            var post = new Post { Id = 1, UserId = "test-user-id" };
            var requestDto = new CreatePostRequest { Text = "Update" };
            var returnedPost = new GetPostResponse { Id = 1, LikesCount = 0, Text = "Updated text", Timestamp = DateTime.Now, UserId = "test-user-id" };
            _mockPostService.Setup(s => s.FindAsync(1)).ReturnsAsync(post);
            _mockPostService.Setup(s => s.UpdatePost(1, requestDto)).ReturnsAsync(returnedPost);

            // Act
            var result = await _postsController.UpdatePost(1, requestDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(returnedPost, okResult.Value);
        }

        [Fact]
        public async Task GetPostFeed_ReturnsPostsFromCache_WhenCacheIsValid()
        {
            // Arrange
            var posts = new List<GetPostResponse>
            {
                new GetPostResponse { Id = 1, Text = "Post 1" },
                new GetPostResponse { Id = 2, Text = "Post 2" }
            };

            _mockCache.Setup(c => c.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetPostResponse>>.IsAny))
                .Callback((string key, out IEnumerable<GetPostResponse> value) =>
                {
                    value = posts;
                })
                .Returns(true);

            // Act
            var result = await _postsController.GetPostFeed();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<GetPostResponse>>(okResult.Value);
            Assert.Equal(posts, model);

            _mockPostService.Verify(s => s.GetPostFeedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            _mockCache.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<IEnumerable<GetPostResponse>>(),
                It.IsAny<MemoryCacheEntryOptions>()), Times.Never);
        }

        [Fact]
        public async Task GetPostFeed_ReturnsPostsFromService_WhenCacheIsInvalid()
        {
            // Arrange
            var posts = new List<GetPostResponse>
            {
                new GetPostResponse { Id = 1, Text = "Post 1" },
                new GetPostResponse { Id = 2, Text = "Post 2" }
            };

            _mockCache.Setup(c => c.TryGetValue(It.IsAny<string>(), out It.Ref<IEnumerable<GetPostResponse>>.IsAny))
                .Returns(false);

            _mockPostService.Setup(s => s.GetPostFeedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(posts);

            // Act
            var result = await _postsController.GetPostFeed();

            // Assert

            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<GetPostResponse>>(okResult.Value);
            Assert.Equal(posts, model);

            _mockPostService.Verify(s => s.GetPostFeedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            _mockCache.Verify(c => c.Set(It.IsAny<string>(), It.IsAny<IEnumerable<GetPostResponse>>(), It.IsAny<MemoryCacheEntryOptions>()), Times.Once);

        }
    }
}
