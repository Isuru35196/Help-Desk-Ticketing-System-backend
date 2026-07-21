using HelpDesk.API.Models;

namespace HelpDesk.API.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(int id);
        Task<IEnumerable<Ticket>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Ticket>> GetByAssignedSupportIdAsync(int assignedSupportId);
        Task AddAsync(Ticket ticket);
        Task UpdateAsync(Ticket ticket);
        Task DeleteAsync(Ticket ticket);
        Task<(IEnumerable<Ticket> Items, int TotalCount)> GetPagedAsync(
            int? userId,
            string? search,
            string? status,
            string? priority,
            int? assignedSupportId,
            string? sort,
            int page,
            int pageSize);
        Task<Dictionary<string, int>> GetTicketCountsAsync(int? userId);
    }
}