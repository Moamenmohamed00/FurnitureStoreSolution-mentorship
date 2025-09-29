using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;

    public UserService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
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

        return new RegisterResultDto
        {
            Succeeded = true,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            }
        };
    }

    public async Task<LoginResultDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            return new LoginResultDto
            {
                Succeeded = false,
                Errors = new List<string> { "Invalid email or password." }
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateJwtToken(user, roles);

        return new LoginResultDto
        {
            Succeeded = true,
            Token = token.Token,
            Expiration = token.Expiration,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email??"",
                PhoneNumber = user.PhoneNumber,
                Roles = roles
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
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            });
        }
        return result;
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

    #region JWT
    private (string Token, DateTime Expiration) GenerateJwtToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddHours(2);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }
    #endregion
}
