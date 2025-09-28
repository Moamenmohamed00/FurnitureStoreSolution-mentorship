using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public User? User { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}
