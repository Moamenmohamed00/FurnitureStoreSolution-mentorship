using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize (Roles = "Admin , Customer")]
    public class CartController : Controller
    {
        private readonly ICartItemService _cartService;

        public CartController(ICartItemService cartService)
        {
            _cartService = cartService;
        }

        // GET: api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var cart = await _cartService.GetCartItemsByUserAsync(userId);
            return Ok(cart);
        }

        // POST: api/cart/items
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] CreateCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            dto.UserId = userId;

            var createdItem = await _cartService.CreateCartItemAsync(dto);
            if (createdItem == null)
                return BadRequest("Failed to add item to cart.");

            return Ok(createdItem); // رجع الـ DTO بدلاً من مجرد رسالة
        }

        // PUT: api/cart/items/{itemId}?newQuantity=5
        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateItemQuantity(int itemId, [FromQuery] int newQuantity)
        {
            if (newQuantity <= 0)
                return BadRequest("Quantity must be greater than zero.");

            var success = await _cartService.UpdateCartItemQuantityAsync(itemId, newQuantity);
            if (!success)
                return NotFound("Cart item not found.");

            var updatedItem = await _cartService.GetCartItemByIdAsync(itemId);
            return Ok(updatedItem);
        }

        // DELETE: api/cart/items/{itemId}
        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var success = await _cartService.DeleteCartItemAsync(itemId);
            if (!success)
                return NotFound("Cart item not found.");

            return Ok($"Item {itemId} removed successfully.");
        }

        // DELETE: api/cart
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var success = await _cartService.ClearCartAsync(userId);
            if (!success)
                return BadRequest("Failed to clear cart.");

            return Ok("Cart cleared successfully.");
        }
    }
}
