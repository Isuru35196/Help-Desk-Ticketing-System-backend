using HelpDesk.API.Data;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HelpDesk.Tests
{
    public class UserRepositoryTests
    {
        private HelpDeskDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<HelpDeskDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new HelpDeskDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            using var context = CreateDbContext();
            var repo = new UserRepository(context);
            var user = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashed",
                Role = "User"
            };

            // Act
            await repo.AddAsync(user);

            // Assert
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(savedUser);
            Assert.Equal("test@example.com", savedUser.Email);
        }

        [Fact]
        public async Task GetByUsernameAsync_ShouldReturnCorrectUser()
        {
            // Arrange
            using var context = CreateDbContext();
            var user = new User
            {
                Username = "anotheruser",
                Email = "another@example.com",
                PasswordHash = "hashed",
                Role = "Support"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repo = new UserRepository(context);

            // Act
            var result = await repo.GetByUsernameAsync("anotheruser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("another@example.com", result.Email);
        }
    }
}
