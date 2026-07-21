using HelpDesk.API.DTOs;
using HelpDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var errorMessage = await _authService.RegisterAsync(dto);
            if (errorMessage != null)
            {
                _logger.LogWarning("Registration failed for username: {Username}. Reason: {Reason}", dto.Username, errorMessage);
                return BadRequest(errorMessage);
            }
            _logger.LogInformation("Successfully registered user: {Username}", dto.Username);
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for user: {UsernameOrEmail}", dto.UsernameOrEmail);
            var response = await _authService.LoginAsync(dto);
            if (response == null)
            {
                _logger.LogWarning("Failed login attempt for user: {UsernameOrEmail}", dto.UsernameOrEmail);
                return Unauthorized("Invalid username or password");
            }
            _logger.LogInformation("Successful login for user: {UsernameOrEmail}", dto.UsernameOrEmail);
            return Ok(response);
        }
    }
}
