using HelpDesk.API.DTOs;
using HelpDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace HelpDesk.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tickets/{id}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ITicketCommentService _commentService;

        public CommentsController(ITicketCommentService commentService)
        {
            _commentService = commentService;
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
        public async Task<IActionResult> AddComment(int id, CreateCommentDto dto)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();
            
            var comment = await _commentService.AddCommentAsync(id, dto, userId, role);
            if (comment == null)
            {
                return NotFound($"Ticket with ID {id} not found or you are not authorized to comment on it.");
            }

            return Ok(new
            {
                comment.Id,
                comment.Comment,
                comment.CreatedAt,
                comment.TicketId,
                comment.UserId
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(int id)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var comments = await _commentService.GetCommentsByTicketIdAsync(id, userId, role);
            if (comments == null)
            {
                return NotFound($"Ticket with ID {id} not found or you are not authorized to view its comments.");
            }

            var result = comments.Select(c => new
            {
                c.Id,
                c.Comment,
                c.CreatedAt,
                c.TicketId,
                c.UserId,
                User = new
                {
                    c.User.Id,
                    c.User.Username,
                    c.User.Role
                }
            });

            return Ok(result);
        }
    }
}
