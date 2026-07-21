using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using HelpDesk.API.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace HelpDesk.Tests
{
    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepoMock;
        private readonly Mock<ITicketHistoryRepository> _historyRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ILogger<TicketService>> _loggerMock;
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            _ticketRepoMock = new Mock<ITicketRepository>();
            _historyRepoMock = new Mock<ITicketHistoryRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<TicketService>>();

            _ticketService = new TicketService(
                _ticketRepoMock.Object,
                _historyRepoMock.Object,
                _userRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task UpdateTicketAsync_ShouldReturnFalse_WhenTicketDoesNotExist()
        {
            // Arrange
            _ticketRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Ticket)null);

            var updateDto = new UpdateTicketDto
            {
                Title = "New Title",
                Description = "New Description",
                Priority = "High",
                Status = "Resolved"
            };

            // Act
            var result = await _ticketService.UpdateTicketAsync(999, updateDto, 1, "Admin");

            // Assert
            Assert.False(result);
            _ticketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTicketAsync_ShouldLogHistory_WhenStatusChanges()
        {
            // Arrange
            var existingTicket = new Ticket
            {
                Id = 1,
                Title = "Old Title",
                Description = "Old Description",
                Priority = "Low",
                Status = "Open",
                UserId = 2
            };

            _ticketRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingTicket);
            _userRepoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new User { Id = 3, Username = "supportguy" });

            var updateDto = new UpdateTicketDto
            {
                Title = "Old Title",
                Description = "Old Description",
                Priority = "Low",
                Status = "Resolved"
            };

            // Act
            var result = await _ticketService.UpdateTicketAsync(1, updateDto, 3, "Support");

            // Assert
            Assert.True(result);
            Assert.Equal("Resolved", existingTicket.Status);
            _historyRepoMock.Verify(r => r.AddAsync(It.Is<TicketHistory>(h => h.OldStatus == "Open" && h.NewStatus == "Resolved" && h.ChangedBy == "supportguy")), Times.Once);
        }
    }
}
