using FurnitureStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.Available;
        //[ForeignKey("User")]
        //public string? UserId { get; set; }
        //public User? User { get; set; }
        //[ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        // [ForeignKey("Brand")]
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }
        public ProductColor? Color { get; set; }
        //[ForeignKey("CreatedBy")]
        public string? CreatedByUserId { get; set; }
        public User? CreatedBy { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
