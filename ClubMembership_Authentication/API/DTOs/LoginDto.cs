using System.ComponentModel.DataAnnotations;

namespace ClubMembership_Authentication.API.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
