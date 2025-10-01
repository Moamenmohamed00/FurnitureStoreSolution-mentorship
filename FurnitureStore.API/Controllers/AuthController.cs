using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
       /* [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
                return BadRequest($"Error from external provider: {remoteError}");

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return RedirectToAction("Login");

            // جرّب تسجيل دخول
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                return Redirect(returnUrl ?? "/");
            }
            else
            {
                // المستخدم أول مرة
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = new ApplicationUser { UserName = email, Email = email };

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    // ربط الحساب الخارجي بالمستخدم
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Redirect(returnUrl ?? "/set-local-password");
                }
                return BadRequest("Could not create user");
            }
        }
        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword(SetPasswordDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _userManager.AddPasswordAsync(user, dto.Password);
            if (result.Succeeded)
                return Ok("Password set successfully");

            return BadRequest(result.Errors);
        }*/

    }
}
