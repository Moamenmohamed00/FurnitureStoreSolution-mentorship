using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
     [ApiController]
     [Route("api/[controller]")]
    [Authorize]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // GET: api/brand
        [HttpGet]
        [AllowAnonymous] // أي حد يقدر يشوف
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        // GET: api/brand/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound($"Brand with Id = {id} not found.");

            return Ok(brand);
        }

        // POST: api/brand
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto createBrandDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var brand = await _brandService.CreateBrandAsync(createBrandDto);
            if (brand == null)
                return BadRequest("Failed to create brand.");

            return Ok(brand);
        }

        // PUT: api/brand/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateBrandDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _brandService.UpdateBrandAsync(id, updateDto);
            if (!success)
                return NotFound($"Brand with Id = {id} not found.");

            return Ok("Brand updated successfully.");
        }

        // DELETE: api/brand/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _brandService.DeleteBrandAsync(id);
            if (!success)
                return NotFound($"Brand with Id = {id} not found.");

            return Ok("Brand deleted successfully.");
        }
    }
}
