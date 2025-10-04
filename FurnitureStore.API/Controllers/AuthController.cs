using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Application.Services;
using FurnitureStore.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static FurnitureStore.Application.DTOs.OAuthen_login_DTOs;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService ;
        private readonly IExternalAuthService _externalAuthService;
        public AuthController(IUserService userService,IRefreshTokenService RefreshTokenService,UserManager<User> userManager,SignInManager<User> signInManager,IJwtService jwtService,IExternalAuthService externalAuth)
        {
            _userService = userService;
            _refreshTokenService =  RefreshTokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService =  jwtService;
            _externalAuthService = externalAuth;
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
                Errors = result.Errors.ToArray(),
                Token = result.Token,
                Expiration = result.Expiration,
                RefreshToken= result.RefreshToken
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
            if (token == null || !token.Succeeded)
                return Unauthorized("Invalid email or password");
           
            return Ok(new { /*Token = token,*/
                Message = "Login successful",
                AccessToken = token.Token,
                AccessTokenExpiration = token.Expiration,
                RefreshToken =token.Token,
                RefreshTokenExpiration = DateTime.UtcNow.AddDays(7),
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
        [HttpPost("external-login")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var result = await _externalAuthService.ExternalLoginAsync(dto, ip);

            if (result == null)
                return BadRequest("External login failed or provider not supported.");

            return Ok(new
            {
                Message = $"{dto.Provider} login successful",
                AccessToken = result.AccessToken,
                AccessTokenExpiration = result.AccessTokenExpiration,
                RefreshToken = result.RefreshToken,
                RefreshTokenExpiration = result.RefreshTokenExpiration,
                User = result.User
            });
        }


        [HttpPost("set-password")]
        public async Task<IActionResult> SetLocalPassword([FromBody] SetLocalPasswordDto dto)
        {
            var result = await _externalAuthService.SetLocalPasswordAsync(dto);
            if (!result)
                return BadRequest("Failed to set password. The user may already have one.");

            return Ok("Password has been set successfully.");
        }

    }
}
