using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;

namespace HelpDesk.API.Repositories.Implementations
{
    public class TicketRepository : ITicketRepository
    {
        private readonly HelpDeskDbContext _context;

        public TicketRepository(HelpDeskDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<Ticket>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}