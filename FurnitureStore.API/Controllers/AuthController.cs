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
        public AuthController(IUserService userService,IRefreshTokenService RefreshTokenService,UserManager<User> userManager,SignInManager<User> signInManager,IJwtService jwtService)
        {
            _userService = userService;
            _refreshTokenService =  RefreshTokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService =  jwtService;
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
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return BadRequest("Google authentication failed.");

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim =>
                    new { claim.Type, claim.Value });

            // email من جوجل
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

            // لو المستخدم مش موجود في Identity، اعمله Create
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FullName = result.Principal.Identity?.Name ?? email
                };
                await _userManager.CreateAsync(user);
                await _userManager.AddToRoleAsync(user, "Customer");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateJwtToken(user, roles);

            return Ok(new
            {
                Message = "Google login successful",
                AccessToken = token.Token,
                AccessTokenExpiration = token.Expiration,
                User = new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Roles = roles
                }
            });
        }

        /*
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
