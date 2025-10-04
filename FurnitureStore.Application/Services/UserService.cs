using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Application.Services;
using FurnitureStore.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJwtService _jwtService;

    public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config,IRefreshTokenService refreshTokenService,IHttpContextAccessor httpContextAccessor,IJwtService jwtService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config; 
        _refreshTokenService = refreshTokenService;
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
    }

    #region Auth
    public async Task<RegisterResultDto> RegisterAsync(RegisterDto registerDto, string role = "Customer")
    {
        var user = new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            PhoneNumber = registerDto.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return new RegisterResultDto
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        await _userManager.AddToRoleAsync(user, role);
        // --- generate access token ---
        var roles = await _userManager.GetRolesAsync(user);
        var jwt = _jwtService.GenerateJwtToken(user, roles);

        // --- generate refresh token and store it ---
        var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var newRefresh = new NewRefreshTokenDto
        {
            Token = refreshTokenString,
            Expires = DateTime.UtcNow.AddDays(7), // change lifetime as needed
            CreatedByIp = ip,
            UserId = user.Id
        };

        await _refreshTokenService.GenerateRefreshTokenAsync(newRefresh);

        return new RegisterResultDto
        {
            Succeeded = true,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            },
            Token = jwt.Token,
            Expiration = jwt.Expiration,
            RefreshToken = refreshTokenString
        };
    }
    

    public async Task<LoginResultDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return new LoginResultDto
            {
                Succeeded = false,
                Errors = new List<string> { "Invalid email or password." }
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtService.GenerateJwtToken(user, roles);
        // rotate/create refresh token
        var refreshTokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var ip = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var newRefresh = new NewRefreshTokenDto
        {
            Token = refreshTokenString,
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ip,
            UserId = user.Id
        };

        // optionally: you could call ReplaceRefreshTokenAsync if you want to revoke old token
        await _refreshTokenService.GenerateRefreshTokenAsync(newRefresh);


        return new LoginResultDto
        {
            Succeeded = true,
            Token = token.Token,
            Expiration = token.Expiration,
            RefreshToken = refreshTokenString,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email??"",
                PhoneNumber = user.PhoneNumber,
                Roles = roles,
                Addresses = user.Addresses?.Select(a => new AddressDto
                {
                    Id = a.Id,
                    Street = a.Street,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.ZipCode,
                    Country = a.Country,
                }).ToList() ?? new List<AddressDto>()
            }
        };
    }
    #endregion

    #region User Utils
    public async Task<UserDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? "",
            PhoneNumber = user.PhoneNumber,
            Roles = roles
        };
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = await Task.WhenAll(users.Select(async u => new {
            User = u,
            Roles = await _userManager.GetRolesAsync(u)
        }));
       return userRoles.Select(ur => new UserDto
        {
            Id = ur.User.Id,
            FullName = ur.User.FullName,
            Email = ur.User.Email ?? "",
            PhoneNumber = ur.User.PhoneNumber,
            Roles = ur.Roles
        });
    }

    public async Task<bool> UpdateAsync(string id, UpdateUserDto updateUserDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;

        user.FullName = updateUserDto.FullName ?? user.FullName;
        user.PhoneNumber = updateUserDto.PhoneNumber ?? user.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> UserExistsAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId) != null;
    }
    #endregion

    #region Roles
    public async Task<bool> IsUserInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AssignRoleToUserAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<bool> RemoveRoleFromUserAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return Enumerable.Empty<string>();

        return await _userManager.GetRolesAsync(user);
    }
    #endregion

  
}
