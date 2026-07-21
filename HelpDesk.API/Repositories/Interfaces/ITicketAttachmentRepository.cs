using HelpDesk.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.API.Repositories.Interfaces
{
    public interface ITicketAttachmentRepository
    {
        Task<TicketAttachment?> GetByIdAsync(int id);
        Task<IEnumerable<TicketAttachment>> GetByTicketIdAsync(int ticketId);
        Task AddAsync(TicketAttachment attachment);
        Task DeleteAsync(TicketAttachment attachment);
    }
}
