using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ✅ Get All Users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // ✅ Get User by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // ✅ Update User
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _userService.UpdateAsync(id, updateUserDto);
            if (!result)
                return BadRequest("Update failed.");

            return Ok("User updated successfully.");
        }

        // ✅ Delete User
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
                return BadRequest("Delete failed.");

            return Ok("User deleted successfully.");
        }

        // ✅ Get User Roles
        [HttpGet("{id}/roles")]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles);
        }

        // ✅ Assign Role
        [HttpPost("{id}/roles/assign")]
        public async Task<IActionResult> AssignRole(string id, [FromQuery] string role)
        {
            var result = await _userService.AssignRoleToUserAsync(id, role);
            if (!result)
                return BadRequest("Assign role failed.");

            return Ok($"Role '{role}' assigned successfully.");
        }

        // ✅ Remove Role
        [HttpPost("{id}/roles/remove")]
        public async Task<IActionResult> RemoveRole(string id, [FromQuery] string role)
        {
            var result = await _userService.RemoveRoleFromUserAsync(id, role);
            if (!result)
                return BadRequest("Remove role failed.");

            return Ok($"Role '{role}' removed successfully.");
        }
    }
}
