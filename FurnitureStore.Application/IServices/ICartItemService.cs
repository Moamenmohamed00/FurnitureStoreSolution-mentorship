using FurnitureStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.IServices
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemDto>> GetCartItemsByUserAsync(string userId);
        Task<CartItemDto?> GetCartItemByIdAsync(int id);
        Task<CartItemDto> CreateCartItemAsync(CreateCartItemDto createCartItemDto);
        Task<bool> UpdateCartItemAsync(int id, CreateCartItemDto createCartItemDto);
        Task<bool> DeleteCartItemAsync(int id);
        Task<bool> ClearCartAsync(string userId); 
        Task<bool> CartItemExistsAsync(string userId, int productId);
        Task<bool> UpdateCartItemQuantityAsync(int id, int newQuantity);
    }
}
