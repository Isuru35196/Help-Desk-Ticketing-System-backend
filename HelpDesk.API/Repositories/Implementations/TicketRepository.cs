using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets.FindAsync(id);
        }

        public async Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId)
        {
            return await _context.Tickets.Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByAssignedSupportIdAsync(int assignedSupportId)
        {
            return await _context.Tickets.Where(t => t.AssignedSupportId == assignedSupportId).ToListAsync();
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Ticket> Items, int TotalCount)> GetPagedAsync(
            int? userId,
            string? search,
            string? status,
            string? priority,
            int? assignedSupportId,
            string? sort,
            int page,
            int pageSize)
        {
            var query = _context.Tickets.AsQueryable();

            // 1. Ownership Filter
            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            // 2. Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(lowerSearch) || 
                                         t.Description.ToLower().Contains(lowerSearch));
            }

            // 3. Filters
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(t => t.Status == status);
            }
            if (!string.IsNullOrWhiteSpace(priority))
            {
                query = query.Where(t => t.Priority == priority);
            }
            if (assignedSupportId.HasValue)
            {
                query = query.Where(t => t.AssignedSupportId == assignedSupportId.Value);
            }

            // 4. Sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var lowerSort = sort.ToLower();
                query = lowerSort switch
                {
                    "priority" => query.OrderBy(t => t.Priority),
                    "status" => query.OrderBy(t => t.Status),
                    "datecreated" => query.OrderBy(t => t.DateCreated),
                    _ => query.OrderByDescending(t => t.DateCreated)
                };
            }
            else
            {
                query = query.OrderByDescending(t => t.DateCreated);
            }

            // 5. Pagination
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Dictionary<string, int>> GetTicketCountsAsync(int? userId)
        {
            var query = _context.Tickets.AsQueryable();
            if (userId.HasValue)
            {
                query = query.Where(t => t.UserId == userId.Value);
            }

            var tickets = await query.ToListAsync();

            return new Dictionary<string, int>
            {
                { "totalTickets", tickets.Count },
                { "openTickets", tickets.Count(t => t.Status.Equals("Open", StringComparison.OrdinalIgnoreCase)) },
                { "closedTickets", tickets.Count(t => t.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase) || t.Status.Equals("Resolved", StringComparison.OrdinalIgnoreCase)) },
                { "pendingTickets", tickets.Count(t => t.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase)) }
            };
        }
    }
}