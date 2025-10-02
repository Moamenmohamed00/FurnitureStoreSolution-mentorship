using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Application.Services;
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
        private readonly IOrderService _orderService;

        public CartController(ICartItemService cartService,IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        // GET: api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            var cart = await _cartService.GetCartItemsByUserAsync(userId);
            return Ok(cart);
        }

        // POST: api/cart/items
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] CreateCartItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var createdItem = await _cartService.CreateCartItemAsync(dto);
            if (createdItem == null)
                return BadRequest("Failed to add item to cart.");

            return Ok(createdItem); 
        }

        // PUT: api/cart/orderitems/{itemId}?newQuantity=5
        [HttpPut("orderitems/{itemId}")]
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
        // POST: api/cart/checkout
        [HttpPost("checkout")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ;

            // 1. هات محتويات الكارت بتاع اليوزر
            var cartItems = await _cartService.GetCartItemsByUserAsync(userId);
            if (!cartItems.Any())
                return BadRequest("Cart is empty.");

            // 2. أنشئ Order جديد
            var createOrderDto = new CreateOrderDto
            {
                UserId = userId,
                ShippingStreet = checkoutDto.ShippingStreet,
                ShippingCity = checkoutDto.ShippingCity,
                ShippingState = checkoutDto.ShippingState,
                ShippingZipCode = checkoutDto.ShippingZipCode,
                ShippingCountry = checkoutDto.ShippingCountry,
                PaymentMethod = checkoutDto.PaymentMethod,
                OrderItems = cartItems.Select(ci => new CreateOrderItemDto
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    TotalPrice = ci.TotalPrice
                }).ToList()
            };

            var order = await _orderService.CreateOrderAsync(createOrderDto);

            await _cartService.ClearCartAsync(userId);

            return Ok(order);
        }

    }
}
