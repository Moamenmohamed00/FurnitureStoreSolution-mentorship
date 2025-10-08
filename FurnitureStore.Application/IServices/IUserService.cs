using FurnitureStore.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.IServices
{
    public interface IUserService
    {
        Task<bool> IsUserInRoleAsync(string userId, string role);
        Task<bool> AssignRoleToUserAsync(string userId, string role);
        Task<bool> RemoveRoleFromUserAsync(string userId, string role);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<bool> UserExistsAsync(string userId);
        Task<UserDto?> GetByIdAsync(string id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<RegisterResultDto> RegisterAsync(RegisterDto registerDto, string role = "Customer");
        Task <LoginResultDto> LoginAsync(LoginDto loginDto);
        Task<bool> UpdateAsync(string id, UpdateUserDto updateUserDto);
        Task<bool> DeleteAsync(string id);
        Task<bool> SendOtpAsync(SendOtpDto dto);
        Task<bool> VerifyOtpAsync(VerifyOtpDto dto);
        Task<IdentityResult> ResetPasswordWithOtpAsync(ResetPasswordDto dto);
    }
}
