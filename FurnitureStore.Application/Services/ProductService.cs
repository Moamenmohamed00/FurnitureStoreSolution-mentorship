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
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IUnitOfWork unitofwork)
        {
            _unitOfWork = unitofwork;
        }
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.UnitPrice,
                Stock = createProductDto.Stock,
                Color = createProductDto.Color,
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Color = product.Color// ?? default(ProductColor) // Explicitly handle nullable ProductColor
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }

           await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Color = p.Color //?? default(ProductColor) // Explicitly handle nullable ProductColor
            });
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Color = product.Color //?? default(ProductColor) // Explicitly handle nullable ProductColor
            };
        }

        public async Task<bool> UpdateProductAsync(int id, CreateProductDto createProductDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }
            product.Name = createProductDto.Name;
            product.Description = createProductDto.Description;
            product.Price = createProductDto.UnitPrice;
            product.Stock = createProductDto.Stock;
            product.Color = createProductDto.Color;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
