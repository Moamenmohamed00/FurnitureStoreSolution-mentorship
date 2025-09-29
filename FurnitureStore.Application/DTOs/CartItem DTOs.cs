using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.DTOs
{
    // Post
    public class CreateCartItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string? UserId { get; set; }
    }

    // Get
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } // للعرض
        public int Quantity { get; set; }
        public string? UserId { get; set; }
    }
}
