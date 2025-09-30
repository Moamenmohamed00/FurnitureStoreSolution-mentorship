using FurnitureStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.IServices
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenDto> GenerateRefreshTokenAsync(NewRefreshTokenDto dto);
        Task<RefreshTokenDto?> GetRefreshTokenAsync(string token);
        Task<bool> ValidateRefreshTokenAsync(string token, string userId);
        Task<bool> RevokeRefreshTokenAsync(string token, string revokedByIp);
        Task<bool> ReplaceRefreshTokenAsync(string oldToken, NewRefreshTokenDto newTokenDto, string replacedByIp);
        Task<int> RemoveExpiredTokensAsync(string userId);
    }
}
