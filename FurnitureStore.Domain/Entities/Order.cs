using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FurnitureStore.Domain.Enums;

namespace FurnitureStore.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }= DateTime.UtcNow;
        public string ShippingStreet { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZipCode { get; set; }
        public Country ShippingCountry { get; set; }
        public PaymentMethod paymentMethod { get; set; }
        public User? User { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
