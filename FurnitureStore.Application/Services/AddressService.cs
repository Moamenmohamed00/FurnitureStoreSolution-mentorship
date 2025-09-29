using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Domain.Entities;
using FurnitureStore.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddressService(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }
        public async Task<AddressDto> CreateAddressAsync(CreateAddressDto createAddressDto)
        {
            var addressDto = new Address()
            {
                Street = createAddressDto.Street,
                City = createAddressDto.City,
                State = createAddressDto.State,
                ZipCode = createAddressDto.PostalCode,
                Country = createAddressDto.Country,
                UserId = createAddressDto.UserId
            };
          await  _unitOfWork.Addresses.AddAsync(addressDto);
           await _unitOfWork.CompleteAsync();
            return new AddressDto
            {
                Id = addressDto.Id,
                Street = addressDto.Street,
                City = addressDto.City,
                State = addressDto.State,
                PostalCode = addressDto.ZipCode,
                Country = addressDto.Country,
                UserId = addressDto.UserId
            };
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (address == null)
            {
                return false;
            }
            await _unitOfWork.Addresses.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<AddressDto?> GetAddressByIdAsync(int id)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (address == null)return null;
            return new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                State = address.State,
                PostalCode = address.ZipCode,
                Country = address.Country,
                UserId = address.UserId
            };
        }

        public async Task<IEnumerable<AddressDto>> GetAllAddressesAsync()
        {
            var addresses = await _unitOfWork.Addresses.GetAllAsync();
            return addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Street = a.Street,
                City = a.City,
                State = a.State,
                PostalCode = a.ZipCode,
                Country = a.Country,
                UserId = a.UserId
            });
        }

        public async Task<bool> UpdateAddressAsync(int id, CreateAddressDto createAddressDto)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (address == null)
            {
                return false;
            }
            address.Street = createAddressDto.Street;
            address.City = createAddressDto.City;
            address.State = createAddressDto.State;
            address.ZipCode = createAddressDto.PostalCode;
            address.Country = createAddressDto.Country;
            address.UserId = createAddressDto.UserId;
            _unitOfWork.Addresses.Update(address);
           await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
