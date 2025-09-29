using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }  //IdentityUser
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PhoneNumber { get; set; }
        public IEnumerable<string>Roles { get; set; }
        public List<OrderDto> Orders { get; set; } = new();
        public List<AddressDto> Addresses { get; set; } = new();
    }
    public class CreateUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }

    }
    public class UpdateUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
