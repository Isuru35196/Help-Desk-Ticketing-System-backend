using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^(Admin|Support|User)$", ErrorMessage = "Role must be 'Admin', 'Support', or 'User'.")]
        public string Role { get; set; }
    }
}
