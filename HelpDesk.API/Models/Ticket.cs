namespace HelpDesk.API.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // e.g., "Open", "Closed", "Pending"
        public string Priority { get; set; } // e.g., "Low", "Medium", "High"
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
        public int? AssignedSupportId { get; set; }
        public User? AssignedSupport { get; set; }
        public DateTime? AssignmentDate { get; set; }
    }
}
