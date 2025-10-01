using FurnitureStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Domain.Entities
{
    public class OrderItem
    {
        //[Key]
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.pending;
    }
}
