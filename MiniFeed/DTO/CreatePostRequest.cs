using System.ComponentModel.DataAnnotations;

namespace MiniFeed.DTO
{
    public class CreatePostRequest
    {
        [Required]
        [Length(1,140, ErrorMessage = "The text cannot be longer than 140 characters.")]
        public string Text { get; set; }
    }
}
