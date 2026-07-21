using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs
{
    public class UpdateUserRoleDto
    {
        [Required]
        [RegularExpression("^(Admin|Support|User)$", ErrorMessage = "Role must be 'Admin', 'Support', or 'User'.")]
        public string Role { get; set; }
    }
}
