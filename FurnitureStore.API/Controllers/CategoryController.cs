using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // ✅ Get All Categories
        [HttpGet]
        [AllowAnonymous] // الكل يقدر يشوف الكاتيجوريز
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // ✅ Get Category by Id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        // ✅ Create Category
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto createCategoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.CreateCategoryAsync(createCategoryDto);
            if (result == null) 
                return BadRequest("Failed to create category.");

            return Ok("Category created successfully.");
        }

        // ✅ Update Category
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCategoryDto updateCategoryDto)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            if (!result)
                return BadRequest("Failed to update category.");

            return Ok("Category updated successfully.");
        }

        // ✅ Delete Category
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return BadRequest("Failed to delete category.");

            return Ok("Category deleted successfully.");
        }
    }
}
