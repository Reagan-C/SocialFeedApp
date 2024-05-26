namespace MiniFeed.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int LikesCount { get; set; } = 0;
        public ICollection<Like> Likes { get; set; } = new HashSet<Like>();

        public User User { get; set; }
    }
}
