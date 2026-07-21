using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using HelpDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HelpDesk.API.Services.Implementations
{
    public class TicketAttachmentService : ITicketAttachmentService
    {
        private readonly ITicketAttachmentRepository _attachmentRepository;
        private readonly ITicketRepository _ticketRepository;

        public TicketAttachmentService(ITicketAttachmentRepository attachmentRepository, ITicketRepository ticketRepository)
        {
            _attachmentRepository = attachmentRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<TicketAttachment?> UploadAttachmentAsync(int ticketId, IFormFile file, int userId, string role, string uploadsFolder)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }

            // Normal users can only add attachments to their own tickets
            if (role == "User" && ticket.UserId != userId)
            {
                return null;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".pdf", ".log", ".txt" };
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Unsupported file type. Supported extensions are: .png, .jpg, .jpeg, .pdf, .log, .txt");
            }

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new TicketAttachment
            {
                TicketId = ticketId,
                FileName = file.FileName,
                FilePath = filePath,
                FileType = file.ContentType,
                DateUploaded = DateTime.UtcNow
            };

            await _attachmentRepository.AddAsync(attachment);
            return attachment;
        }

        public async Task<IEnumerable<TicketAttachment>?> GetAttachmentsByTicketIdAsync(int ticketId, int userId, string role)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }

            if (role == "User" && ticket.UserId != userId)
            {
                return null;
            }

            return await _attachmentRepository.GetByTicketIdAsync(ticketId);
        }

        public async Task<TicketAttachment?> GetAttachmentByIdAsync(int id, int userId, string role)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment == null)
            {
                return null;
            }

            var ticket = await _ticketRepository.GetByIdAsync(attachment.TicketId);
            if (ticket == null)
            {
                return null;
            }

            if (role == "User" && ticket.UserId != userId)
            {
                return null;
            }

            return attachment;
        }
    }
}
