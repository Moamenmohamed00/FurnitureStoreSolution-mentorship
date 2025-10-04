using FurnitureStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.DTOs
{
    public class OAuthen_login_DTOs
    {
        public class ExternalLoginDto
        {
            [Required]
            public ExternalLoginProvider Provider { get; set; }  // "Google"
            [Required]
            public string IdToken { get; set; } = string.Empty;  // Google ID Token
        }

        public class ExternalLoginResultDto
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime AccessTokenExpiration { get; set; }
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime RefreshTokenExpiration { get; set; }
            public UserDto User { get; set; }
            public ExternalLoginProvider Provider { get; set; }
        }

        public class SetLocalPasswordDto
        {

            [Required]
            public string UserId { get; set; } = string.Empty;
            [Required]
            [MinLength(6,ErrorMessage ="password must be at least 6 charcaters")]
            public string NewPassword { get; set; } = string.Empty;
        }

    }
}
