using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.DTOs
{
    public class RefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && DateTime.UtcNow < Expires;
    }

    public class NewRefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public string CreatedByIp { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
    public class JwtAuthResultDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }

}
