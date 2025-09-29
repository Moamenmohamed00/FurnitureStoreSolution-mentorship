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
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartItemService(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }
        public async Task<CartItemDto> CreateCartItemAsync(CreateCartItemDto createCartItemDto)
        {
            var cartItem = new Domain.Entities.CartItem
            {
                ProductId = createCartItemDto.ProductId,
                Quantity = createCartItemDto.Quantity,
                UserId = createCartItemDto.UserId
            };
            await _unitOfWork.CartItems.AddAsync(cartItem);
            await _unitOfWork.CompleteAsync();
            return new CartItemDto
            {
                Id = cartItem.Id,
                ProductName = cartItem.Product?.Name ?? "",
                ProductId = cartItem.ProductId??0,
                Quantity = cartItem.Quantity,
                UserId = cartItem.UserId
            };
        }

        public async Task<bool> DeleteCartItemAsync(int id)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null)
            {
                return false;
            }
            await _unitOfWork.CartItems.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<CartItemDto>> GetCartItemsByUserAsync(string userId)
        {
            var items = await _unitOfWork.CartItems.FindAsync(c => c.UserId == userId);
            return items.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductName = ci.Product?.Name ?? "",
                ProductId = ci.ProductId??0,
                Quantity = ci.Quantity,
                UserId = ci.UserId
            });
        }

        public async Task<CartItemDto?> GetCartItemByIdAsync(int id)
        {
            var cartItem =await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null)
            {
                return null;
            }
            return new CartItemDto
            {
                Id = cartItem.Id,
                ProductName = cartItem.Product?.Name ?? "",
                ProductId = cartItem.ProductId??0,
                Quantity = cartItem.Quantity,
                UserId = cartItem.UserId
            };
        }

        public async Task<bool> UpdateCartItemAsync(int id, CreateCartItemDto createCartItemDto)
        {
            var cartItem =await _unitOfWork.CartItems.GetByIdAsync(id);
            if (cartItem == null)
            {
                return false;
            }
            cartItem.ProductId = createCartItemDto.ProductId;
            cartItem.Quantity = createCartItemDto.Quantity;
            cartItem.UserId = createCartItemDto.UserId;
            _unitOfWork.CartItems.Update(cartItem);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
