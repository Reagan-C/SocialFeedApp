namespace MiniFeed.Models
{
    public class Follow
    {
        public string FollowerId { get; set; }
        public User Follower { get; set; }

        public string FollowedId { get; set; }
        public User Followed { get; set; }
    }
}
