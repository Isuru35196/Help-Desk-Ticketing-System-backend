using HelpDesk.API.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpDesk.API.Services.Interfaces
{
    public interface ITicketAttachmentService
    {
        Task<TicketAttachment?> UploadAttachmentAsync(int ticketId, IFormFile file, int userId, string role, string uploadsFolder);
        Task<IEnumerable<TicketAttachment>?> GetAttachmentsByTicketIdAsync(int ticketId, int userId, string role);
        Task<TicketAttachment?> GetAttachmentByIdAsync(int id, int userId, string role);
    }
}
