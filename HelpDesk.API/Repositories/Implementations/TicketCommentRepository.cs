using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDesk.API.Repositories.Implementations
{
    public class TicketCommentRepository : ITicketCommentRepository
    {
        private readonly HelpDeskDbContext _context;

        public TicketCommentRepository(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task<TicketComment?> GetByIdAsync(int id)
        {
            return await _context.TicketComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<TicketComment>> GetByTicketIdAsync(int ticketId)
        {
            return await _context.TicketComments
                .Include(c => c.User)
                .Where(c => c.TicketId == ticketId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(TicketComment comment)
        {
            _context.TicketComments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TicketComment comment)
        {
            _context.TicketComments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }
}
