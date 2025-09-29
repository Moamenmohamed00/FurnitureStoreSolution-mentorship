using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/order
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto createOrderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // جلب الـ UserId من الـ Token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            createOrderDto.UserId = userId;

            var order = await _orderService.CreateOrderAsync(createOrderDto);
            return Ok(order);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            // لو مش Admin تأكد إن الأوردر بتاع نفس اليوزر
            if (!User.IsInRole("Admin") && order.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            return Ok(order);
        }

        // GET: api/order/user
        [HttpGet("user")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetUserOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return Ok(orders);
        }

        // GET: api/order
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateOrderDto updateDto)
        {
            var success = await _orderService.UpdateOrderAsync(id, updateDto);
            if (!success)
                return NotFound();

            return Ok("Order updated successfully.");
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success)
                return NotFound();

            return Ok("Order deleted successfully.");
        }

        // GET: api/order/{id}/items
        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetOrderItems(int id)
        {
            var items = await _orderService.GetOrderItemsByOrderIdAsync(id);
            return Ok(items);
        }

        // POST: api/order/{id}/items
        [HttpPost("{id}/items")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddItem(int id, [FromBody] CreateOrderItemDto dto)
        {
            var success = await _orderService.AddOrderItemAsync(id, dto);
            if (!success)
                return BadRequest("Failed to add item to order.");

            return Ok("Item added successfully.");
        }

        // PUT: api/order/items/{itemId}
        [HttpPut("items/{itemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateItemQuantity(int itemId, [FromQuery] int newQuantity)
        {
            var success = await _orderService.UpdateOrderItemQuantityAsync(itemId, newQuantity);
            if (!success)
                return BadRequest("Failed to update item quantity.");

            return Ok("Item quantity updated successfully.");
        }

        // DELETE: api/order/items/{itemId}
        [HttpDelete("items/{itemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            var success = await _orderService.DeleteOrderItemAsync(itemId);
            if (!success)
                return NotFound("Order item not found.");

            return Ok("Order item deleted successfully.");
        }
    }
}
