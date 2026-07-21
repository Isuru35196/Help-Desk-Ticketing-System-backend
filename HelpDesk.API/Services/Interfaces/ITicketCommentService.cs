using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.API.Services.Interfaces
{
    public interface ITicketCommentService
    {
        Task<TicketComment?> AddCommentAsync(int ticketId, CreateCommentDto dto, int userId, string role);
        Task<IEnumerable<TicketComment>?> GetCommentsByTicketIdAsync(int ticketId, int userId, string role);
    }
}
