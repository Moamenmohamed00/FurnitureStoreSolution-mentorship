using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Domain.Entities;
using FurnitureStore.Domain.Enums;
using FurnitureStore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.Services
{
    public class OrderItemService:IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddOrderItemAsync(int orderId, CreateOrderItemDto orderItemDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;
            var product = await _unitOfWork.Products.GetByIdAsync(orderItemDto.ProductId);
            if (product == null) return false;
            // Map CreateOrderItemDto to OrderItem entity
            var orderItem = new OrderItem
            {
                OrderId = orderId,
                ProductId = orderItemDto.ProductId,
                Quantity = orderItemDto.Quantity,
                UnitPrice = product.Price,
                OrderStatus = OrderStatus.pending
            };
            order.TotalPrice += orderItem.UnitPrice * orderItem.Quantity;
            await _unitOfWork.OrderItems.AddAsync(orderItem);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteOrderItemAsync(int id)
        {
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem == null) return false;

            // Use null-coalescing operator to handle nullable OrderId
            var order = await _unitOfWork.Orders.GetByIdAsync(orderItem.OrderId);
            if (order == null) return false;

            order.TotalPrice -= orderItem.UnitPrice * orderItem.Quantity;
            await _unitOfWork.OrderItems.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<OrderItemDto?> GetOrderItemByIdAsync(int orderItemId)
        {
            var item = await _unitOfWork.OrderItems.GetByIdAsync(orderItemId, oi => oi.Product);
            OrderItemDto? result = null;
            if (item != null)
            {
                result = MapOrderItemToDto(item);
            }
            return result;
        }

      
        public async Task<bool> OrderExistsAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            return order != null;
        }

        public async Task<bool> UpdateOrderItemAsync(int orderItemId, CreateOrderItemDto updateOrderItemDto)
        {
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(orderItemId);
            if (orderItem == null) return false;

            var order = await _unitOfWork.Orders.GetByIdAsync(orderItem.OrderId);
            if (order == null) return false;

            // تعديل السعر الإجمالي
            order.TotalPrice -= orderItem.UnitPrice * orderItem.Quantity;

            orderItem.ProductId = updateOrderItemDto.ProductId;
            orderItem.Quantity = updateOrderItemDto.Quantity;

            order.TotalPrice += orderItem.UnitPrice * orderItem.Quantity;

            _unitOfWork.OrderItems.Update(orderItem);
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity)
        {
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(orderItemId);
            if (orderItem == null || newQuantity <= 0) return false;
            var order = await _unitOfWork.Orders.GetByIdAsync(orderItem.OrderId);
            if (order == null) return false;
            order.TotalPrice -= orderItem.UnitPrice * orderItem.Quantity;
            orderItem.Quantity = newQuantity;
            order.TotalPrice += orderItem.UnitPrice * orderItem.Quantity;
            _unitOfWork.OrderItems.Update(orderItem);
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        private OrderItemDto MapOrderItemToDto(OrderItem item)
        {
            return new OrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId ?? 0,
                ProductName = item.Product?.Name ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                OrderId = item.OrderId,
                OrderStatus = item.OrderStatus
            };
        }
    }
}
