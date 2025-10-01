using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FurnitureStore.Application.DTOs.OAuthen_login_DTOs;

namespace FurnitureStore.Application.IServices
{
    public interface IExternalAuthService
    {
        Task<ExternalLoginResultDto?> ExternalLoginAsync(ExternalLoginDto externalLoginDto, string ipAddress);
        Task<bool> SetLocalPasswordAsync(SetLocalPasswordDto setLocalPasswordDto);
    }
}
