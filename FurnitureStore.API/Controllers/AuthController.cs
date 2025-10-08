using Azure.Core;
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
using System.Net.Mail;
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
        private readonly ILogger<AuthController> _logger;
        public AuthController(IUserService userService,IRefreshTokenService RefreshTokenService,UserManager<User> userManager,SignInManager<User> signInManager,IJwtService jwtService,IExternalAuthService externalAuth,ILogger<AuthController> logger)
        {
            _userService = userService;
            _refreshTokenService =  RefreshTokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService =  jwtService;
            _externalAuthService = externalAuth;
            _logger = logger;
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
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
        {
            if (!ModelState.IsValid) // Let model validation handle bad input
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userService.SendOtpAsync(dto);
                if (!result)
                {
                    // User not found, could be 404 or a generic message for security
                    return NotFound("User with the specified email not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending OTP to {Email}", dto.Email);
                // Return a 500 for server-side errors
                return StatusCode(500, "An internal error occurred while trying to send the OTP.");
            }
            return Ok(new { Message = "OTP has been sent successfully to your email." });
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var isValid = await _userService.VerifyOtpAsync(dto);
            if (!isValid)
                return BadRequest("Invalid or expired OTP.");

            return Ok(new { Message = "OTP verified successfully. You can now reset your password." });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var result = await _userService.ResetPasswordWithOtpAsync(dto);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { Message = "Password has been reset successfully." });
        }

    }
}
