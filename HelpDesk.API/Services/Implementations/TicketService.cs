using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using HelpDesk.API.Services.Interfaces;

namespace HelpDesk.API.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task CreateTicketAsync(CreateTicketDto dto)
        {
            var ticket = new Ticket
            {
                Title = dto.Title,
                Status = "Open"
            };

            await _ticketRepository.AddAsync(ticket);
        }
    }
}