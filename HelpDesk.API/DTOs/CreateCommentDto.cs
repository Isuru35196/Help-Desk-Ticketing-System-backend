using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs
{
    public class CreateCommentDto
    {
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Comment { get; set; }
    }
}
