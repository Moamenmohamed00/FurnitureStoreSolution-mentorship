using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public Task<bool> AddOrderItemAsync(int orderId, CreateOrderItemDto orderItemDto)
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrderAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrderItemAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderAsync(int id, CreateOrderDto updateOrderDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity)
        {
            throw new NotImplementedException();
        }
    }
}
