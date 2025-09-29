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
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BrandService(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }
        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto createBrandDto)
        {
           var brand = new Domain.Entities.Brand
            {
                Name = createBrandDto.Name
            };
             await _unitOfWork.Brands.AddAsync(brand);
             await _unitOfWork.CompleteAsync();
            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
            {
                return false;
            }
            await _unitOfWork.Brands.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.Brands.GetAllAsync();
            return brands.Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name
            });
        }

        public async Task<BrandDto?> GetBrandByIdAsync(int id)
        {
            var brand =await  _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
            {
                return null;
            }
            return new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name
            };
        }

        public async Task<bool> UpdateBrandAsync(int id, CreateBrandDto createBrandDto)
        {
            var brand = await _unitOfWork.Brands.GetByIdAsync(id);
            if (brand == null)
            {
                return false;
            }
            brand.Name = createBrandDto.Name;
            _unitOfWork.Brands.Update(brand);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
