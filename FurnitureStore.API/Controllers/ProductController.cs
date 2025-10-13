using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/product
        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound("Product not found.");

            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var uploadfolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadfolder))
            {
                Directory.CreateDirectory(uploadfolder);
            }
            var fileName = $"{Guid.NewGuid()}_{createProductDto.Imagefile.FileName}";
            var filePath = Path.Combine(uploadfolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await createProductDto.Imagefile.CopyToAsync(stream);
            }
            createProductDto.ImageUrl = $"/uploads/{fileName}";
            var created = await _productService.CreateProductAsync(createProductDto);
            if (created == null)
                return BadRequest("Failed to create product.");

            return Ok(new { 
            message= "Product created successfully.",
            data= created
            });

        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _productService.UpdateProductAsync(id, updateProductDto);
            if (!success)
                return NotFound("Product not found.");

            return Ok("Product updated successfully.");
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
                return NotFound("Product not found.");

            return Ok("Product deleted successfully.");
        }
    }
}

