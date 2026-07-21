using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using HelpDesk.API.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace HelpDesk.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _configMock = new Mock<IConfiguration>();
            _authService = new AuthService(_userRepoMock.Object, _configMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnErrorMessage_WhenUsernameIsTaken()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "takenuser",
                Email = "new@example.com",
                Password = "password123",
                Role = "User"
            };

            _userRepoMock.Setup(r => r.GetByUsernameAsync("takenuser"))
                .ReturnsAsync(new User { Username = "takenuser" });

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.Equal("Username is already taken.", result);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRegisterSuccessfully_WhenInputIsCorrect()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "password123",
                Role = "User"
            };

            _userRepoMock.Setup(r => r.GetByUsernameAsync("newuser")).ReturnsAsync((User)null);
            _userRepoMock.Setup(r => r.GetByEmailAsync("new@example.com")).ReturnsAsync((User)null);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.Null(result);
            _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Username == "newuser" && u.Email == "new@example.com")), Times.Once);
        }
    }
}
