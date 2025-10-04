using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Domain.Entities;
using FurnitureStore.Domain.Enums;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FurnitureStore.Application.DTOs.OAuthen_login_DTOs;

namespace FurnitureStore.Application.Services
{
    public class ExternalAuthService : IExternalAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly RoleManager<IdentityRole> _roleManager ;

        public ExternalAuthService(UserManager<User> userManager, IJwtService jwtService, IRefreshTokenService refreshTokenService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _roleManager = roleManager;
        }

        public async Task<ExternalLoginResultDto?> ExternalLoginAsync(ExternalLoginDto externalLoginDto, string ipAddress)
        {
            if (externalLoginDto.Provider != ExternalLoginProvider.Google)
                return null;

            var payload = await GoogleJsonWebSignature.ValidateAsync(externalLoginDto.IdToken);
            if (payload == null) return null;

            var provider = externalLoginDto.Provider.ToString();
            var providerKey = payload.Subject;

            var user = await _userManager.FindByLoginAsync(provider, providerKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        FullName = payload.Name ?? payload.Email.Split('@')[0],
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded) return null;
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!await _roleManager.RoleExistsAsync("Customer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }
                    await _userManager.AddToRoleAsync(user, "Customer");
                }

                if (!await _roleManager.RoleExistsAsync("Customer"))
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));

                if (!await _userManager.IsInRoleAsync(user, "Customer"))
                    await _userManager.AddToRoleAsync(user, "Customer");

                var existingLogin = (await _userManager.GetLoginsAsync(user))
                    .FirstOrDefault(l => l.LoginProvider == provider && l.ProviderKey == providerKey);

                if (existingLogin == null)
                    await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtService.GenerateJwtToken(user, roles);
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(new NewRefreshTokenDto
            {
                UserId = user.Id,
                CreatedByIp = ipAddress
            });

            return new ExternalLoginResultDto
            {
                AccessToken = accessToken.Token,
                AccessTokenExpiration = accessToken.Expiration,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.Expires,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                }
            };
        }

        

        public async Task<bool> SetLocalPasswordAsync(SetLocalPasswordDto setLocalPasswordDto)
        {
            var user = await _userManager.FindByIdAsync(setLocalPasswordDto.UserId);
            if (user == null) return false;

            // لو كان عنده Password قديم → لازم نمنعه إلا لو بيعمل Reset
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword) return false;

            var result = await _userManager.AddPasswordAsync(user, setLocalPasswordDto.NewPassword);
            return result.Succeeded;
        }
    }
}
