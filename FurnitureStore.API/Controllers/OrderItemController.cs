using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : Controller
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        // GET: api/orderitem/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _orderItemService.GetOrderItemByIdAsync(id);
            if (item == null)
                return NotFound("Order item not found.");

            return Ok(item);
        }

        // Updated Create method to include the required orderId parameter for AddOrderItemAsync
        [HttpPost("{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int orderId, [FromBody] CreateOrderItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderExists = await _orderItemService.OrderExistsAsync(orderId);
            if (!orderExists)
                return NotFound("Order not found.");

            var created = await _orderItemService.AddOrderItemAsync(orderId, dto);
            if (!created)
                return BadRequest("Failed to create order item.");

            return Ok("Order item created successfully.");
        }
        // Corrected the casing of the type name to match the expected convention
        [HttpPut("{id}")]

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Update(int id, [FromBody] CreateOrderItemDto dto)
        {

            if (!ModelState.IsValid)

                return BadRequest(ModelState);



            var success = await _orderItemService.UpdateOrderItemAsync(id, dto);

            if (!success)

                return NotFound("Order item not found.");



            return Ok("Order item updated successfully.");

        }


       


        // PUT: api/order/items/{itemId}
        [HttpPut("items/{itemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateItemQuantity(int itemId, [FromQuery] int newQuantity)
        {
            var success = await _orderItemService.UpdateOrderItemQuantityAsync(itemId, newQuantity);
            if (!success)
                return BadRequest("Failed to update item quantity.");

            return Ok("Item quantity updated successfully.");
        }

        // DELETE: api/orderitem/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _orderItemService.DeleteOrderItemAsync(id);
            if (!success)
                return NotFound("Order item not found.");

            return Ok("Order item deleted successfully.");
        }
    }
}
