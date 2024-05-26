using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MiniFeed.Data;
using MiniFeed.DTO;
using MiniFeed.Interfaces;
using MiniFeed.Models;
using MiniFeed.Repositories;

namespace MiniFeed.Services
{
    public class PostService : IPostService
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PostService(IUserRepository userRepository, AppDbContext context, IMapper mapper)
        {
            _userRepository=userRepository;
            _context=context;
            _mapper=mapper;
        }

        public async Task<GetPostResponse> CreatePost(string userId, CreatePostRequest request)
        {
            var user = _userRepository.GetUserById(userId).GetAwaiter().GetResult();
            var post = _mapper.Map<Post>(request);
            post.User = user;
            _context.Posts.Add(post);
            _context.SaveChangesAsync();
            return _mapper.Map<GetPostResponse>(post);
        }

        public async void DeletePost(int postId)
        {
            var post = _context.Posts.FindAsync(postId).GetAwaiter().GetResult();
            _context.Posts.Remove(post);
            _context.SaveChangesAsync();
        }

        public async Task<Post> FindAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            return post;
        }


        public async Task<GetPostResponse> GetPostById(int postId)
        {
            var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId);
            return _mapper.Map<GetPostResponse>(post);
        }

        public async Task<IEnumerable<GetPostResponse>> GetPostFeedAsync(int pageSize, int page, string currentUserId)
        {
            var currentUser = await _context.Users.FindAsync(currentUserId);
            // Get the user's followed users' IDs
            var followedUserIds = await _context.Follows
                .Where(f => f.FollowerId == currentUserId)
                .Select(f => f.FollowedId)
                .ToListAsync();

            // Include the current user's ID in the followed user IDs
            followedUserIds.Add(currentUserId);

            var posts = await _context.Posts
               .Where(p => followedUserIds.Contains(p.UserId))
               .OrderByDescending(p => p.LikesCount)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
                return _mapper.Map<IEnumerable<GetPostResponse>>(posts);
        }

        public async Task<GetPostResponse> UpdatePost(int postId, CreatePostRequest request)
        {
            var post = await _context.Posts.FindAsync(postId);
            post.Text = request.Text;
            await _context.SaveChangesAsync();
            return _mapper.Map<GetPostResponse>(post);
        }
    }
}
