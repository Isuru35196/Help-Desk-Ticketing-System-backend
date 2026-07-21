using HelpDesk.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.API.Repositories.Interfaces
{
    public interface ITicketCommentRepository
    {
        Task<TicketComment?> GetByIdAsync(int id);
        Task<IEnumerable<TicketComment>> GetByTicketIdAsync(int ticketId);
        Task AddAsync(TicketComment comment);
        Task DeleteAsync(TicketComment comment);
    }
}
