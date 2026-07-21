using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using HelpDesk.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.API.Services.Implementations
{
    public class TicketCommentService : ITicketCommentService
    {
        private readonly ITicketCommentRepository _commentRepository;
        private readonly ITicketRepository _ticketRepository;

        public TicketCommentService(ITicketCommentRepository commentRepository, ITicketRepository ticketRepository)
        {
            _commentRepository = commentRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<TicketComment?> AddCommentAsync(int ticketId, CreateCommentDto dto, int userId, string role)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }

            // Normal users can only comment on their own tickets
            if (role == "User" && ticket.UserId != userId)
            {
                return null;
            }

            var comment = new TicketComment
            {
                TicketId = ticketId,
                UserId = userId,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            return comment;
        }

        public async Task<IEnumerable<TicketComment>?> GetCommentsByTicketIdAsync(int ticketId, int userId, string role)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }

            // Normal users can only view comments on their own tickets
            if (role == "User" && ticket.UserId != userId)
            {
                return null;
            }

            return await _commentRepository.GetByTicketIdAsync(ticketId);
        }
    }
}
