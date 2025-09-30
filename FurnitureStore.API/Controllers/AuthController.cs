using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserService _userService;
        public AuthController(IUserService userService,IRefreshTokenService RefreshTokenService)
        {
            _userService = userService;
            _refreshTokenService =  RefreshTokenService;
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
            // RefreshToken
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(new NewRefreshTokenDto
            {
                UserId = token.User!.Id,
                CreatedByIp = ipAddress
            });

            return Ok(new { /*Token = token,*/
                Message = "Login successful",
                AccessToken = token.Token,
                AccessTokenExpiration = token.Expiration,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.Expires,
                User = token.User
            });
        }
        [HttpPost("logout")]
        [Authorize] 
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _refreshTokenService.RevokeRefreshTokenAsync(dto.RefreshToken, ipAddress);

            if (!result)
                return BadRequest("Invalid or already revoked refresh token.");

            return Ok("Logged out successfully.");
        }
    }
}
