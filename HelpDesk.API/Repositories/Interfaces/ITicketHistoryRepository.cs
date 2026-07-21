using HelpDesk.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.API.Repositories.Interfaces
{
    public interface ITicketHistoryRepository
    {
        Task AddAsync(TicketHistory history);
        Task<IEnumerable<TicketHistory>> GetByTicketIdAsync(int ticketId);
    }
}
