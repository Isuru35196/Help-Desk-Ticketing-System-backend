using HelpDesk.API.DTOs;
using HelpDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HelpDesk.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tickets")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim ?? "0");
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTicketDto dto)
        {
            var userId = GetCurrentUserId();
            await _ticketService.CreateTicketAsync(dto, userId);
            return Ok("Ticket created");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TicketQueryParameters query)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            var result = await _ticketService.GetPagedTicketsAsync(query, userId, role);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            var ticket = await _ticketService.GetTicketByIdAsync(id, userId, role);
            if (ticket == null)
            {
                return NotFound($"Ticket with ID {id} not found or you are not authorized to view it.");
            }
            return Ok(ticket);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTicketDto dto)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            var success = await _ticketService.UpdateTicketAsync(id, dto, userId, role);
            if (!success)
            {
                return NotFound($"Ticket with ID {id} not found or you are not authorized to update it.");
            }
            return Ok("Ticket updated");
        }

        [HttpGet("assigned")]
        [Authorize(Roles = "Admin,Support")]
        public async Task<IActionResult> GetAssigned()
        {
            var userId = GetCurrentUserId();
            var tickets = await _ticketService.GetAssignedTicketsAsync(userId);
            return Ok(tickets);
        }

        [HttpPut("{id}/assign")]
        [Authorize(Roles = "Admin,Support")]
        public async Task<IActionResult> Assign(int id, AssignTicketDto dto)
        {
            var success = await _ticketService.AssignTicketAsync(id, dto.AssignedSupportId);
            if (!success)
            {
                return NotFound($"Ticket with ID {id} not found.");
            }
            return Ok("Ticket assigned successfully.");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _ticketService.DeleteTicketAsync(id);
            if (!success)
            {
                return NotFound($"Ticket with ID {id} not found.");
            }
            return Ok("Ticket deleted");
        }

        [HttpGet("{id}/history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHistory(int id)
        {
            var history = await _ticketService.GetTicketHistoryAsync(id);
            return Ok(history);
        }

        [HttpGet("dashboard/stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            var stats = await _ticketService.GetDashboardStatsAsync(userId, role);
            return Ok(stats);
        }
    }
}