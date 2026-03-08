using HelpDesk.API.DTOs;

namespace HelpDesk.API.Services.Interfaces
{
    public interface ITicketService
    {
        Task CreateTicketAsync(CreateTicketDto dto);
    }
}