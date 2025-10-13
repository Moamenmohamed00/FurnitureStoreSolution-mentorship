using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Domain.Entities;
using FurnitureStore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RefreshTokenService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<RefreshTokenDto> GenerateRefreshTokenAsync(NewRefreshTokenDto dto)
        {
            var refreshToken = new RefreshToken
            {
                Token = dto.Token??Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = dto.Expires,
                Created = DateTime.UtcNow,
                CreatedByIp = dto.CreatedByIp,
                UserId = dto.UserId
            };
          await  _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.CompleteAsync();
            
        return MapToDto(refreshToken);
        }

        public async Task<RefreshTokenDto?> GetRefreshTokenAsync(string token)
        {
            var refreshToken = (await _unitOfWork.RefreshTokens.FindAsync(rt => rt.Token == token)).FirstOrDefault();
            if (refreshToken == null) return null;
           return MapToDto(refreshToken);

        }

        public async Task<int> RemoveExpiredTokensAsync(string userId)
        {
            var expiredTokens =(await _unitOfWork.RefreshTokens
                .FindAsync(rt => rt.UserId == userId && rt.Expires <= DateTime.UtcNow)).ToList();
            if (!expiredTokens.Any()) return 0;
            _unitOfWork.RefreshTokens.DeleteRange(expiredTokens);
           return await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> ReplaceRefreshTokenAsync(string oldToken, NewRefreshTokenDto newTokenDto, string replacedByIp)
        {
            var existingToken = (await _unitOfWork.RefreshTokens
                .FindAsync(rt => rt.Token == oldToken)).FirstOrDefault();
            if (existingToken == null ||! existingToken.IsActive) return false;
            // Mark old token as revoked
            existingToken.Revoked = DateTime.UtcNow;
            existingToken.RevokedByIp = replacedByIp;
            existingToken.ReplacedByToken = newTokenDto.Token;

            _unitOfWork.RefreshTokens.Update(existingToken);

            // Create new refresh token
            var newRefreshToken = new RefreshToken
            {
                Token = newTokenDto.Token,
                Expires = newTokenDto.Expires,
                Created = DateTime.UtcNow,
                CreatedByIp = newTokenDto.CreatedByIp,
                UserId = newTokenDto.UserId
            };
            await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token, string revokedByIp)
        {
            var refreshToken =(await _unitOfWork.RefreshTokens.FindAsync(rt => rt.Token == token)).FirstOrDefault();
            if (refreshToken == null || refreshToken.Revoked!=null) return false;
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = revokedByIp;
            _unitOfWork.RefreshTokens.Update(refreshToken);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, string userId)
        {
            var refreshToken = (await _unitOfWork.RefreshTokens.FindAsync(rt => rt.Token == token && rt.UserId == userId)).FirstOrDefault();
            return refreshToken != null && refreshToken.IsActive/*&&refreshToken.Expires>DateTime.UtcNow*/;

        }
        // Helper method to map RefreshToken entity to RefreshTokenDto
        private static RefreshTokenDto MapToDto(RefreshToken rt) => new RefreshTokenDto
        {
            Token = rt.Token,
            Expires = rt.Expires,
            Created = rt.Created,
            CreatedByIp = rt.CreatedByIp,
            Revoked = rt.Revoked,
            RevokedByIp = rt.RevokedByIp
        };

    }
}
