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

        public ExternalAuthService(UserManager<User> userManager, IJwtService jwtService, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ExternalLoginResultDto?> ExternalLoginAsync(ExternalLoginDto externalLoginDto, string ipAddress)
        {
          if (externalLoginDto.Provider != "Google") return null;

    // ✅ تحقق من Google Token
    var payload = await GoogleJsonWebSignature.ValidateAsync(externalLoginDto.IdToken);
    if (payload == null) return null;

    var email = payload.Email;
    var user = await _userManager.FindByEmailAsync(email);

    if (user == null)
    {
        user = new User
        {
            UserName = email,
            Email = email,
            FullName = payload.Name
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded) return null;

        // ✅ اربط الحساب بـ Google
        await _userManager.AddLoginAsync(user, new UserLoginInfo(
            externalLoginDto.Provider,
            payload.Subject, // Google UserId
            externalLoginDto.Provider
        ));
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
            FullName = user.FullName
        }
    };
        }

        public  async Task<bool> SetLocalPasswordAsync(SetLocalPasswordDto setLocalPasswordDto)
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
