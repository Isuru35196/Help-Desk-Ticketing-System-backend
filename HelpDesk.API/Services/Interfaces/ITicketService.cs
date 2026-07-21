using HelpDesk.API.DTOs;
using HelpDesk.API.Models;

namespace HelpDesk.API.Services.Interfaces
{
    public interface ITicketService
    {
        Task CreateTicketAsync(CreateTicketDto dto, int userId);
        Task<IEnumerable<Ticket>> GetAllTicketsAsync(int userId, string role);
        Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(int supportUserId);
        Task<Ticket?> GetTicketByIdAsync(int id, int userId, string role);
        Task<bool> UpdateTicketAsync(int id, UpdateTicketDto dto, int userId, string role);
        Task<bool> AssignTicketAsync(int id, int? supportUserId);
        Task<bool> DeleteTicketAsync(int id);
        Task<IEnumerable<TicketHistory>> GetTicketHistoryAsync(int ticketId);
        Task<PaginatedResultDto<Ticket>> GetPagedTicketsAsync(TicketQueryParameters queryParams, int userId, string role);
        Task<Dictionary<string, int>> GetDashboardStatsAsync(int userId, string role);
    }
}