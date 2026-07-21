using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDesk.API.Repositories.Implementations
{
    public class TicketAttachmentRepository : ITicketAttachmentRepository
    {
        private readonly HelpDeskDbContext _context;

        public TicketAttachmentRepository(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task<TicketAttachment?> GetByIdAsync(int id)
        {
            return await _context.TicketAttachments.FindAsync(id);
        }

        public async Task<IEnumerable<TicketAttachment>> GetByTicketIdAsync(int ticketId)
        {
            return await _context.TicketAttachments
                .Where(a => a.TicketId == ticketId)
                .ToListAsync();
        }

        public async Task AddAsync(TicketAttachment attachment)
        {
            _context.TicketAttachments.Add(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TicketAttachment attachment)
        {
            _context.TicketAttachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }
    }
}
