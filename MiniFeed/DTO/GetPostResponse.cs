namespace MiniFeed.DTO
{
    public class GetPostResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; } 
        public int LikesCount { get; set; }
    }
}
