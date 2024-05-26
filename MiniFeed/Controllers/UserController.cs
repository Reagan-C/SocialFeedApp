using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MiniFeed.Interfaces;
using MiniFeed.Models;
using MiniFeed.Repositories;
using System.Security.Claims;

namespace MiniFeed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private const string FeedCacheKey = "PostFeed_{0}_{1}_{2}";
        private const int pageSize = 10;
        private const int page = 1;
        private readonly IUserRepository _userRepo;
        private readonly IFollowRepository _followRepo;
        private readonly ILikeRepository _likeRepo;
        private readonly IPostService _postService;
        private readonly IMemoryCache _cache;

        public UserController(IUserRepository userRepo, IPostService postService,
            IFollowRepository followRepo, IMemoryCache cache, ILikeRepository likeRepo)
        {
            _userRepo=userRepo;
            _postService=postService;
            _followRepo=followRepo;
            _cache=cache;
            _likeRepo=likeRepo;
        }

        [HttpPost("{username}/follow")]
        public async Task<IActionResult> FollowUser(string username)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser =  await _userRepo.GetUserById(currentUserId);

            var userToFollow = await _userRepo.GetUserAsync(username);
            if (userToFollow == null)
            {
                return NotFound("User not found");
            }

            if (currentUser.UserName == username)
            {
                return BadRequest("You cannot follow yourself.");
            }
            var existingFollow = await _followRepo.GetFollow(currentUserId, userToFollow.Id);
            if (existingFollow != null)
            {
                return BadRequest("Already following the user");
            }
            var follow = new Follow
            {
                FollowerId = currentUserId,
                FollowedId = userToFollow.Id
            };
            await _followRepo.AddFollow(follow);

            //invalidate cache
            var cacheKey = string.Format(FeedCacheKey, currentUserId, page, pageSize);
            _cache.Remove(cacheKey);
            return Ok("User followed successfully");
        }

        [HttpPost("{username}/unfollow")]
        public async Task<IActionResult> UnfollowUser(string username)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser = await _userRepo.GetUserById(currentUserId);

            var userToUnfollow = await _userRepo.GetUserAsync(username);
            if (userToUnfollow == null)
            {
                return NotFound("User not found");
            }

            if (currentUser.UserName == username)
            {
                return BadRequest("You cannot unfollow yourself.");
            }

            var existingFollow = await _followRepo.GetFollow(currentUserId, userToUnfollow.Id);
            if (existingFollow == null)
            {
                return BadRequest("You are not following this user");
            }

            await _followRepo.UnFollow(existingFollow);

            return Ok("User unfollowed successfully");
        }

        [HttpPost("{postId}/like")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var post = await _postService.FindAsync(postId);

            if (post == null)
            {
                return NotFound("Post not found");
            }

            // Check if the user has already liked the post
            var existingLike = await _likeRepo.GetLike(currentUserId, post.Id);

            if (existingLike != null)
            {
                return BadRequest("You have already liked this post");
            }

            // like post if user hasnt liked it
            var userLike = new Like
            {
                UserId = currentUserId,
                PostId = postId
            };
            post.LikesCount++;
            await _likeRepo.AddLike(userLike);
            
            return Ok("Post liked successfully");
        }
    }
}
