using FurnitureStore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.IServices
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAllAddressesAsync();
        Task<AddressDto?> GetAddressByIdAsync(int id);
        Task<AddressDto> CreateAddressAsync(CreateAddressDto createAddressDto);
        Task<bool> UpdateAddressAsync(int id, CreateAddressDto createAddressDto);
        Task<bool> DeleteAddressAsync(int id);
    }
}
