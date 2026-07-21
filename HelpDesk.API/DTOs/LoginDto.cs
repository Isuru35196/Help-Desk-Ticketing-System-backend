using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
