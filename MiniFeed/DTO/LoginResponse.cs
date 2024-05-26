namespace MiniFeed.DTO
{
    public class LoginResponse
    {
        public RetrievedUserResponse UserResponse { get; set; }
        public string Token { get; set; }
    }
}
