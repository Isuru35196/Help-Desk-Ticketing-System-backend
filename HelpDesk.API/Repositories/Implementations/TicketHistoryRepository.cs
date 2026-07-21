using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDesk.API.Repositories.Implementations
{
    public class TicketHistoryRepository : ITicketHistoryRepository
    {
        private readonly HelpDeskDbContext _context;

        public TicketHistoryRepository(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TicketHistory history)
        {
            _context.TicketHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TicketHistory>> GetByTicketIdAsync(int ticketId)
        {
            return await _context.TicketHistories
                .Where(h => h.TicketId == ticketId)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }
    }
}
