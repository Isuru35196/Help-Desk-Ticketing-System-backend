using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HelpDesk.Tests
{
    public class TicketRepositoryTests
    {
        private HelpDeskDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<HelpDeskDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new HelpDeskDbContext(options);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldFilterAndPaginateTickets()
        {
            // Arrange
            using var context = CreateDbContext();
            var ticket1 = new Ticket { Title = "Auth Login Issue", Description = "Can't log in", Status = "Open", Priority = "High", DateCreated = DateTime.UtcNow };
            var ticket2 = new Ticket { Title = "Dashboard Bug", Description = "Graph not rendering", Status = "Pending", Priority = "Medium", DateCreated = DateTime.UtcNow };
            var ticket3 = new Ticket { Title = "Printer Issue", Description = "Out of toner", Status = "Open", Priority = "Low", DateCreated = DateTime.UtcNow };
            
            context.Tickets.AddRange(ticket1, ticket2, ticket3);
            await context.SaveChangesAsync();

            var repo = new TicketRepository(context);

            // Act
            var (items, totalCount) = await repo.GetPagedAsync(
                userId: null,
                search: "issue",
                status: "Open",
                priority: null,
                assignedSupportId: null,
                sort: "datecreated",
                page: 1,
                pageSize: 10
            );

            // Assert
            Assert.Equal(2, totalCount);
            Assert.Equal(2, items.Count());
            Assert.Contains(items, t => t.Title == "Auth Login Issue");
            Assert.Contains(items, t => t.Title == "Printer Issue");
        }
    }
}
