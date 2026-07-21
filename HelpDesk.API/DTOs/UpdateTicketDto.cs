using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.DTOs
{
    public class UpdateTicketDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Description { get; set; }

        [Required]
        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Priority must be 'Low', 'Medium', or 'High'.")]
        public string Priority { get; set; }

        [Required]
        [RegularExpression("^(Open|Pending|Resolved|Closed)$", ErrorMessage = "Status must be 'Open', 'Pending', 'Resolved', or 'Closed'.")]
        public string Status { get; set; }
    }
}
