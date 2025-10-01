using FurnitureStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.DTOs
{
    public class OAuthen_login_DTOs
    {
        public class ExternalLoginDto
        {
            public string Provider { get; set; }  // "Google"
            public string IdToken { get; set; } = string.Empty;  // Google ID Token
        }

        public class ExternalLoginResultDto
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime AccessTokenExpiration { get; set; }
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime RefreshTokenExpiration { get; set; }
            public UserDto User { get; set; }
        }

        public class SetLocalPasswordDto
        {
            public string UserId { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }

    }
}
