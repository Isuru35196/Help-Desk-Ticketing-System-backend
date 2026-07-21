using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using HelpDesk.API.Services.Interfaces;

using Microsoft.Extensions.Logging;

namespace HelpDesk.API.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketHistoryRepository _historyRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TicketService> _logger;

        public TicketService(
            ITicketRepository ticketRepository, 
            ITicketHistoryRepository historyRepository,
            IUserRepository userRepository,
            ILogger<TicketService> logger)
        {
            _ticketRepository = ticketRepository;
            _historyRepository = historyRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task CreateTicketAsync(CreateTicketDto dto, int userId)
        {
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = "Open",
                UserId = userId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            };

            await _ticketRepository.AddAsync(ticket);
            _logger.LogInformation("Ticket created successfully. TicketId: {TicketId}, Title: {Title}, CreatedByUserId: {UserId}", ticket.Id, ticket.Title, userId);
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(int userId, string role)
        {
            if (role == "Admin" || role == "Support")
            {
                return await _ticketRepository.GetAllAsync();
            }
            return await _ticketRepository.GetByUserIdAsync(userId);
        }

        public async Task<Ticket?> GetTicketByIdAsync(int id, int userId, string role)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return null;
            }

            if (role == "User" && ticket.UserId != userId)
            {
                // Forbidden access, return null to act as NotFound or deny access
                return null;
            }

            return ticket;
        }

        public async Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(int supportUserId)
        {
            return await _ticketRepository.GetByAssignedSupportIdAsync(supportUserId);
        }

        public async Task<bool> UpdateTicketAsync(int id, UpdateTicketDto dto, int userId, string role)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return false;
            }

            var oldStatus = ticket.Status;

            // User role can only edit their own OPEN tickets and cannot change the status
            if (role == "User")
            {
                if (ticket.UserId != userId)
                {
                    return false;
                }
                if (ticket.Status != "Open")
                {
                    return false;
                }
                
                ticket.Title = dto.Title;
                ticket.Description = dto.Description;
                ticket.Priority = dto.Priority;
            }
            else // Admin/Support can update anything including status
            {
                ticket.Title = dto.Title;
                ticket.Description = dto.Description;
                ticket.Priority = dto.Priority;
                ticket.Status = dto.Status;
            }

            ticket.DateUpdated = DateTime.UtcNow;

            await _ticketRepository.UpdateAsync(ticket);

            _logger.LogInformation("Ticket updated successfully. TicketId: {TicketId}, UpdatedByUserId: {UserId}", ticket.Id, userId);

            // Log status change if there is any
            if (oldStatus != ticket.Status)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                var changer = user?.Username ?? "Unknown";

                var history = new TicketHistory
                {
                    TicketId = ticket.Id,
                    ChangedBy = changer,
                    OldStatus = oldStatus,
                    NewStatus = ticket.Status,
                    ChangedAt = DateTime.UtcNow
                };

                await _historyRepository.AddAsync(history);
            }

            return true;
        }

        public async Task<bool> AssignTicketAsync(int id, int? supportUserId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                _logger.LogWarning("Failed to assign ticket. Ticket with ID {TicketId} not found.", id);
                return false;
            }

            ticket.AssignedSupportId = supportUserId;
            ticket.AssignmentDate = supportUserId.HasValue ? DateTime.UtcNow : null;
            ticket.DateUpdated = DateTime.UtcNow;

            await _ticketRepository.UpdateAsync(ticket);
            _logger.LogInformation("Ticket assignment updated. TicketId: {TicketId}, AssignedSupportId: {AssignedSupportId}", ticket.Id, supportUserId);
            return true;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                _logger.LogWarning("Failed to delete ticket. Ticket with ID {TicketId} not found.", id);
                return false;
            }

            await _ticketRepository.DeleteAsync(ticket);
            _logger.LogInformation("Ticket deleted successfully. TicketId: {TicketId}", id);
            return true;
        }

        public async Task<IEnumerable<TicketHistory>> GetTicketHistoryAsync(int ticketId)
        {
            return await _historyRepository.GetByTicketIdAsync(ticketId);
        }

        public async Task<PaginatedResultDto<Ticket>> GetPagedTicketsAsync(TicketQueryParameters queryParams, int userId, string role)
        {
            int? filterUserId = (role == "User") ? userId : null;

            var (items, totalCount) = await _ticketRepository.GetPagedAsync(
                filterUserId,
                queryParams.Search,
                queryParams.Status,
                queryParams.Priority,
                queryParams.AssignedSupportId,
                queryParams.Sort,
                queryParams.Page,
                queryParams.PageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / queryParams.PageSize);

            return new PaginatedResultDto<Ticket>
            {
                Items = items,
                TotalCount = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize,
                TotalPages = totalPages
            };
        }

        public async Task<Dictionary<string, int>> GetDashboardStatsAsync(int userId, string role)
        {
            int? filterUserId = (role == "User") ? userId : null;
            return await _ticketRepository.GetTicketCountsAsync(filterUserId);
        }
    }
}