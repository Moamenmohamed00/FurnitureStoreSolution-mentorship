using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Domain.Entities;
using FurnitureStore.Domain.Enums;
using FurnitureStore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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


        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            if (createOrderDto == null)
                throw new ArgumentNullException(nameof(createOrderDto));

            if (createOrderDto.OrderItems == null || !createOrderDto.OrderItems.Any())
                throw new Exception("Order must contain at least one item");

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
                TotalPrice = 0,
                 OrderItems = new List<OrderItem>()
            };

            decimal computedTotal = 0;
            foreach (var item in createOrderDto.OrderItems)
            {
                if (item.Quantity <= 0)
                    throw new Exception($"Quantity for product {item.ProductId} must be greater than zero");

                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with Id {item.ProductId} not found");

                if (item.Quantity > product.Stock)
                    throw new Exception($"Not enough stock for product {product.Name}. Available: {product.Stock}, requested: {item.Quantity}");

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price,
                    OrderStatus = OrderStatus.pending
                };

                order.OrderItems.Add(orderItem);
                computedTotal += orderItem.UnitPrice * orderItem.Quantity;

                // تحديث المخزون بعد الخصم
                product.Stock -= item.Quantity;
                _unitOfWork.Products.Update(product);
            }

            order.TotalPrice = computedTotal;

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return await GetOrderByIdAsync(order.Id) ?? throw new Exception("Error in creating order");
        }


        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId, o => o.OrderItems);
            if (order == null) return false;
            // Restock items before deleting the order
            foreach (var item in order.OrderItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId ?? 0);
                if (product != null)
                {
                    product.Stock += item.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }
           await _unitOfWork.Orders.DeleteAsync(orderId);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.Orders
                .Query()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            return orders.Select(MapOrderToDto);
        }
        
        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders
                .Query()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;
            return MapOrderToDto(order);
        }

      

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
                var orders = await _unitOfWork.Orders.Query().Include(o=>o.OrderItems).ThenInclude(oi=>oi.Product).Where(o => o.UserId == userId).ToListAsync();
                foreach (var order in orders)
                {
                    if (order.OrderItems == null) continue;
                    foreach (var item in order.OrderItems)
                    {
                        if (item.Product == null && item.ProductId.HasValue)
                        {
                            item.Product = await _unitOfWork.Products.GetByIdAsync(item.ProductId.Value);
                        }
                    }
                }
                return orders.Select(MapOrderToDto);
        }
  public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var order = await _unitOfWork.Orders
                .Query()
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return Enumerable.Empty<OrderItemDto>();
            return (order.OrderItems ?? new List<OrderItem>()).Select(MapOrderItemToDto);
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
                OrderItems = (order.OrderItems ?? new List<OrderItem>()).Select(MapOrderItemToDto).ToList()
            };
        }
    }
}
