using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Domain.Entities
{
    [Index(nameof(Token), IsUnique = true)] // Moved the Index attribute to the class level
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        //[Index(IsUnique =true)]
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedByIp { get; set; } = string.Empty;
        public DateTime? Revoked { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;
        [NotMapped]
        public bool IsRevoked => Revoked != null;
        [NotMapped]
        public bool IsActive => !IsRevoked && !IsExpired;
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
