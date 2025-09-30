using FurnitureStore.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FurnitureStore.Application.DTOs
{
    // post
    public class CreateAddressDto
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string? PostalCode { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Country Country { get; set; }

        public string UserId { get; set; } // ربط العنوان بالمستخدم
    }

    // get
    public class AddressDto
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string? PostalCode { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Country Country { get; set; }

        public string UserId { get; set; }
    }
}
