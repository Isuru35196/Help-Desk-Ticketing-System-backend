using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDesk.API.DTOs;
using HelpDesk.API.Models;
using HelpDesk.API.Repositories.Interfaces;
using HelpDesk.API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HelpDesk.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            var existingUserByUsername = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existingUserByUsername != null)
            {
                return "Username is already taken.";
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUserByEmail != null)
            {
                return "Email is already registered.";
            }

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "User" : dto.Role
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);
            return null;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            User? user = await _userRepository.GetByUsernameAsync(dto.UsernameOrEmail);
            if (user == null)
            {
                user = await _userRepository.GetByEmailAsync(dto.UsernameOrEmail);
            }

            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "SuperSecretKeyForHelpDeskSystemThatIsLongEnoughToBeValidSignature123456!";
            var issuer = _configuration["Jwt:Issuer"] ?? "HelpDeskAPI";
            var audience = _configuration["Jwt:Audience"] ?? "HelpDeskClient";
            var expireMinutesStr = _configuration["Jwt:ExpireMinutes"] ?? "60";
            if (!int.TryParse(expireMinutesStr, out var expireMinutes))
            {
                expireMinutes = 60;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
