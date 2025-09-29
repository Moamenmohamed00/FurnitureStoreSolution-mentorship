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
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<bool> AddOrderItemAsync(int orderId, CreateOrderItemDto orderItemDto)
        {
            var order =await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;
            // Map CreateOrderItemDto to OrderItem entity
            var orderItem = new Domain.Entities.OrderItem
            {
                OrderId = orderId,
                ProductId = orderItemDto.ProductId,
                Quantity = orderItemDto.Quantity,
                UnitPrice = orderItemDto.UnitPrice,
                OrderStatus=OrderStatus.pending
            };
            order.TotalPrice += orderItem.UnitPrice * orderItem.Quantity;
            await _unitOfWork.OrderItems.AddAsync(orderItem);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var order = new Order
            {
                UserId = createOrderDto.UserId,
                OrderDate = DateTime.UtcNow,
                ShippingStreet = createOrderDto.ShippingStreet,
                ShippingCity = createOrderDto.ShippingCity,
                ShippingState = createOrderDto.ShippingState,
                ShippingZipCode = createOrderDto.ShippingZipCode,
                ShippingCountry = createOrderDto.ShippingCountry,
                paymentMethod = createOrderDto.PaymentMethod,
            };
            foreach(var item in createOrderDto.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    OrderStatus=OrderStatus.pending
                };
                order.OrderItems.Add(orderItem);
                order.TotalPrice += orderItem.UnitPrice * orderItem.Quantity;
            }
           await _unitOfWork.Orders.AddAsync(order);
           await _unitOfWork.CompleteAsync();
            // Map Order entity to OrderDto
            return await GetOrderByIdAsync(order.Id) ?? throw new Exception("Error in creating order");
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order =await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null) return false;
           await _unitOfWork.Orders.DeleteAsync(orderId);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteOrderItemAsync(int id)
        {
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);
            if (orderItem == null) return false;

            // Use null-coalescing operator to handle nullable OrderId
            var order = await _unitOfWork.Orders.GetByIdAsync(orderItem.OrderId ?? 0);
            if (order == null) return false;

            order.TotalPrice -= orderItem.UnitPrice * orderItem.Quantity;
            await _unitOfWork.OrderItems.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders.GetAllAsync();

            // Map the list of Order entities to a list of OrderDto
            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId??"",
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                ShippingStreet = order.ShippingStreet,
                ShippingCity = order.ShippingCity,
                ShippingState = order.ShippingState,
                ShippingZipCode = order.ShippingZipCode,
                ShippingCountry = order.ShippingCountry,
                PaymentMethod = order.paymentMethod,
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId??0,
                    ProductName = item.Product?.Name ?? string.Empty,
                    OrderId = item.OrderId??0,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    OrderStatus = item.OrderStatus
                }).ToList()
            });

            return orderDtos;
        }
        public async Task<bool> OrderExistsAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            return order != null;
        }
        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {

                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null) return null;
                return MapOrderToDto(order);
           
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
        {

                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null) return Enumerable.Empty<OrderItemDto>();
                return order.OrderItems.Select(MapOrderItemToDto);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
                var orders = await _unitOfWork.Orders.FindAsync(o => o.UserId == userId);
                return orders.Select(MapOrderToDto);
        }

        public async Task<bool> UpdateOrderAsync(int id, CreateOrderDto updateOrderDto)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null) return false;
            order.ShippingStreet = updateOrderDto.ShippingStreet;
            order.ShippingCity = updateOrderDto.ShippingCity;
            order.ShippingState = updateOrderDto.ShippingState;
            order.ShippingZipCode = updateOrderDto.ShippingZipCode;
            order.ShippingCountry = updateOrderDto.ShippingCountry;
            order.paymentMethod = updateOrderDto.PaymentMethod;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
        return true;
        }

        public async Task<bool> UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity)
        {
            var orderItem =await  _unitOfWork.OrderItems.GetByIdAsync(orderItemId);
            if (orderItem == null || newQuantity <= 0) return false;
            var order = await _unitOfWork.Orders.GetByIdAsync(orderItem.OrderId ?? 0);
            if (order == null) return false;
            order.TotalPrice -= orderItem.UnitPrice * orderItem.Quantity;
            orderItem.Quantity = newQuantity;
            order.TotalPrice += orderItem.UnitPrice * orderItem.Quantity;
            _unitOfWork.OrderItems.Update(orderItem);
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<bool> UpdateOrderItemAsync(int orderItemId, CreateOrderItemDto updateOrderItemDto)
        {
            var orderItem = await _unitOfWork.OrderItems.GetByIdAsync(orderItemId);
            if (orderItem == null) return false;

            var order = await _unitOfWork.Orders.GetByIdAsync(orderItem.OrderId ?? 0);
            if (order == null) return false;

            // تعديل السعر الإجمالي
            order.TotalPrice -= orderItem.UnitPrice * orderItem.Quantity;

            orderItem.ProductId = updateOrderItemDto.ProductId;
            orderItem.Quantity = updateOrderItemDto.Quantity;
            orderItem.UnitPrice = updateOrderItemDto.UnitPrice;

            order.TotalPrice += orderItem.UnitPrice * orderItem.Quantity;

            _unitOfWork.OrderItems.Update(orderItem);
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        public async Task<OrderItemDto?> GetOrderItemByIdAsync(int orderItemId)
        {
            var item = await _unitOfWork.OrderItems.GetByIdAsync(orderItemId);
            return item == null ? null : MapOrderItemToDto(item);
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
                OrderId = item.OrderId ?? 0,
                OrderStatus = item.OrderStatus
            };
        }
        private OrderDto MapOrderToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId ?? string.Empty,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                ShippingStreet = order.ShippingStreet,
                ShippingCity = order.ShippingCity,
                ShippingState = order.ShippingState,
                ShippingZipCode = order.ShippingZipCode,
                ShippingCountry = order.ShippingCountry,
                PaymentMethod = order.paymentMethod,
                OrderItems = order.OrderItems.Select(MapOrderItemToDto).ToList()
            };
        }
    }
}
