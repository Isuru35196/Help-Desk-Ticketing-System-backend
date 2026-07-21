using HelpDesk.API.DTOs;
using HelpDesk.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            // Projecting to avoid sending password hashes
            var userList = users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email,
                u.Role
            });
            return Ok(userList);
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, UpdateUserRoleDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            if (dto.Role != "Admin" && dto.Role != "Support" && dto.Role != "User")
            {
                return BadRequest("Invalid role. Role must be Admin, Support, or User.");
            }

            user.Role = dto.Role;
            await _userRepository.UpdateAsync(user);

            return Ok($"User role updated to {dto.Role}");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            await _userRepository.DeleteAsync(user);
            return Ok("User deleted successfully");
        }
    }
}
