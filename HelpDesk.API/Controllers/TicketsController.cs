using HelpDesk.API.DTOs;
using HelpDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTicketDto dto)
        {
            await _ticketService.CreateTicketAsync(dto);
            return Ok("Ticket created");
        }
    }
}