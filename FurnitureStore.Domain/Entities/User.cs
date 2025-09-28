using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
namespace FurnitureStore.Domain.Entities
{
    public class User: IdentityUser
    {
        //public string Id { get; set; }= Guid.NewGuid().ToString();
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        // Navigation properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        //public ICollection<Product> PurchasedProducts { get; set; } = new List<Product>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Product> ProductsCreated { get; set; } = new List<Product>();
    }
}

