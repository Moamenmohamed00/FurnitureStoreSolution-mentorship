using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FurnitureStore.Domain.Enums;

namespace FurnitureStore.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public Country Country { get; set; }
        public User User { get; set; }
        //[ForeignKey("User")]
        public string UserId { get; set; }
        
    }
}
