using System.ComponentModel.DataAnnotations;

namespace MiniFeed.DTO
{
    public class RetrievedUserResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
