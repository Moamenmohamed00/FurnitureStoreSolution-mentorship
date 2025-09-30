using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, [FromQuery] string role = "Customer")
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RegisterAsync(registerDto, role);
            if (!result.Succeeded) 
                return BadRequest(result.Errors);
            var dto = new RegisterResultDto
            {
                User = result.User,
                Succeeded = result.Succeeded,
                Errors = result.Errors.ToArray()
            };
            // return Ok($"{dto}\n User registered successfully.");
            return Ok(new
            {
                Message = "User registered successfully.",
                Data = dto
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _userService.LoginAsync(loginDto);
            if (token == null)
                return Unauthorized("Invalid login attempt.");

            return Ok(new { Token = token });
        }
    }
}
