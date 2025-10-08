using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.DTOs
{
    public class RegisterDto
    {
        
        [Required,MaxLength(100)]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Name must contain only letters and spaces.")]
        public string FullName { get; set; } = string.Empty;
        
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)] // Fixed the issue by replacing [password] with [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string PasswordConfirmed { get; set; } = string.Empty;
        public CreateAddressDto? Address { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)] 
        public required string Password { get; set; }
    }
    public class RegisterResultDto
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();
        public UserDto? User { get; set; }



        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
    }
    public class LoginResultDto
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
        public UserDto? User { get; set; }
        public IEnumerable<string> Errors { get; set; } = new List<string>();

        public string? RefreshToken { get; set; }
    }
    public class LogoutDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
