using MiniFeed.DTO;
using MiniFeed.Models;

namespace MiniFeed.Interfaces
{
    public interface IPostService
    {
        Task<GetPostResponse> CreatePost(string userId, CreatePostRequest request);
        Task<GetPostResponse> GetPostById(int postId);
        void DeletePost(int postId);
        Task<Post> FindAsync(int postId);
        Task<GetPostResponse> UpdatePost(int postId, CreatePostRequest request);
        Task<IEnumerable<GetPostResponse>> GetPostFeedAsync(int pageSize, int page, string currentUserId);
    }
}
