using FurnitureStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.IServices
{
    public interface IOrderService
    {
      
        // Orders
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<bool> UpdateOrderAsync(int id, CreateOrderDto updateOrderDto);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<bool> DeleteOrderAsync(int orderId);

        // Order Items
        Task<bool> AddOrderItemAsync(int orderId, CreateOrderItemDto orderItemDto);
        Task<bool> DeleteOrderItemAsync(int id);
        Task<bool> UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity);
        Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId);
    }
}
