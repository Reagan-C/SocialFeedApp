using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MiniFeed.DTO;
using MiniFeed.Interfaces;
using System.Security.Claims;

namespace MiniFeed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private const string FeedCacheKey = "PostFeed_{0}_{1}_{2}";
        private const int pageSize = 10;
        private const int page = 1;
        private readonly IPostService _postService;
        private readonly IMemoryCacheWrapper _cache;

        public PostsController(IPostService postService, IMemoryCacheWrapper cache)
        {
            _postService=postService;
            _cache=cache;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var post = await _postService.CreatePost(userId, model);

                return CreatedAtRoute(nameof(GetPost), new { id = post.Id }, post);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}", Name = "GetPost")]
        [Authorize]
        public async Task<IActionResult> GetPost(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _postService.GetPostById(id);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            return Ok(post);
        }

        [HttpDelete("{postId}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(int postId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _postService.FindAsync(postId);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (post.UserId != currentUserId)
            {
                return Forbid("You are not authorized to delete this post");
            }

            _postService.DeletePost(postId);

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePostRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _postService.FindAsync(id);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (post.UserId != currentUserId)
            {
                return Forbid("You are not authorized to update this post");
            }

            var updatedPost = await _postService.UpdatePost(id, model);

            return Ok(updatedPost);
        }

        [HttpGet("feed")]
        [Authorize]
        public async Task<IActionResult> GetPostFeed()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var cacheKey = string.Format(FeedCacheKey, currentUserId, page, pageSize);

            if (_cache.TryGetValue(cacheKey, out IEnumerable<GetPostResponse> posts))
            {
                return Ok(posts);
            }
            posts = await _postService.GetPostFeedAsync(pageSize, page, currentUserId);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));

            _cache.Set(cacheKey, posts, cacheEntryOptions);

            return Ok(posts);
        }
    }
}
