using HelpDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace HelpDesk.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tickets/{id}/attachments")]
    public class AttachmentsController : ControllerBase
    {
        private readonly ITicketAttachmentService _attachmentService;
        private readonly IWebHostEnvironment _env;

        public AttachmentsController(ITicketAttachmentService attachmentService, IWebHostEnvironment env)
        {
            _attachmentService = attachmentService;
            _env = env;
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            // Store files inside project directory: "Uploads"
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads");

            try
            {
                var attachment = await _attachmentService.UploadAttachmentAsync(id, file, userId, role, uploadsFolder);
                if (attachment == null)
                {
                    return NotFound($"Ticket with ID {id} not found or you are not authorized to upload attachments to it.");
                }

                return Ok(new
                {
                    attachment.Id,
                    attachment.FileName,
                    attachment.FileType,
                    attachment.DateUploaded,
                    attachment.TicketId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttachments(int id)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var attachments = await _attachmentService.GetAttachmentsByTicketIdAsync(id, userId, role);
            if (attachments == null)
            {
                return NotFound($"Ticket with ID {id} not found or you are not authorized to view its attachments.");
            }

            var result = attachments.Select(a => new
            {
                a.Id,
                a.FileName,
                a.FileType,
                a.DateUploaded,
                a.TicketId
            });

            return Ok(result);
        }

        [HttpGet("{attachmentId}")]
        public async Task<IActionResult> DownloadAttachment(int id, int attachmentId)
        {
            var userId = GetCurrentUserId();
            var role = GetCurrentUserRole();

            var attachment = await _attachmentService.GetAttachmentByIdAsync(attachmentId, userId, role);
            if (attachment == null || attachment.TicketId != id)
            {
                return NotFound($"Attachment with ID {attachmentId} not found or you are not authorized to access it.");
            }

            if (!System.IO.File.Exists(attachment.FilePath))
            {
                return NotFound("The physical file does not exist on the server.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(attachment.FilePath);
            return File(fileBytes, attachment.FileType, attachment.FileName);
        }
    }
}
