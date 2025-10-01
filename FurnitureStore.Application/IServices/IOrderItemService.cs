using FurnitureStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.IServices
{
    public interface IOrderItemService
    {
        // Order Items
        Task<bool> AddOrderItemAsync(int orderId, CreateOrderItemDto orderItemDto);
        Task<bool> DeleteOrderItemAsync(int id);
        Task<bool> UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity);
        Task<OrderItemDto?> GetOrderItemByIdAsync(int orderItemId);
        Task<bool> UpdateOrderItemAsync(int orderItemId, CreateOrderItemDto updateOrderItemDto);
        Task<bool> OrderExistsAsync(int orderId);
    }
}
