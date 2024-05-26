using Microsoft.AspNetCore.Identity;

namespace MiniFeed.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Follow> UsersFollowed { get; set; } = new HashSet<Follow>();
        public ICollection<Follow> Followers { get; set; } = new HashSet<Follow>();
        public ICollection<Like> LikedPosts { get; set; } = new HashSet<Like>();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
