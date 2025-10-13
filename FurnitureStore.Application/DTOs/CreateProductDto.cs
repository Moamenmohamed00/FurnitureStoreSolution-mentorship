using FurnitureStore.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.DTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage = "Image is required")]
        public IFormFile Imagefile { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
        public string? Color { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
    }
}
