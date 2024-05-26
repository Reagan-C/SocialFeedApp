using System.ComponentModel.DataAnnotations;

namespace MiniFeed.DTO
{
    public class AssignRoleRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string RoleName { get; set; }
    }
}
