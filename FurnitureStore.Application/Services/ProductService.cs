using FurnitureStore.Application.DTOs;
using FurnitureStore.Application.IServices;
using FurnitureStore.Domain.Entities;
using FurnitureStore.Domain.Enums;
using FurnitureStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
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
            if (string.IsNullOrWhiteSpace(createProductDto.Name))
                throw new ArgumentException("Product name is required.");

            if (createProductDto.UnitPrice < 0)
                throw new ArgumentException("Price cannot be negative.");

            if (createProductDto.Stock < 0)
                throw new ArgumentException("Stock cannot be negative.");
            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.UnitPrice,
                Stock = createProductDto.Stock,
                Color = createProductDto.Color,
               CategoryId=createProductDto.CategoryId,
                BrandId=createProductDto.BrandId,
                ImageUrl= createProductDto.ImageUrl
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            product = await _unitOfWork.Products
    .Query()
    .Include(p => p.Category)
    .Include(p => p.Brand)
    .FirstOrDefaultAsync(p => p.Id == product.Id);
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Color = product.Color,// ?? default(ProductColor) // Explicitly handle nullable ProductColor
                BrandId=product.BrandId,
                CategoryId=product.CategoryId,
                CategoryName=product.Category?.Name,
                BrandName=product.Brand?.Name,

            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            product = await _unitOfWork.Products
.Query()
.Include(p => p.Category)
.Include(p => p.Brand)
.FirstOrDefaultAsync(p => p.Id == product.Id);
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
            var products =  _unitOfWork.Products
    .Query()
    .Include(p => p.Category)
    .Include(p => p.Brand)
    .ToList();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                Color = p.Color, //?? default(ProductColor) // Explicitly handle nullable ProductColor
                CategoryName=p.Category?.Name,
                BrandName=p.Brand?.Name,
                BrandId=p.BrandId,
                CategoryId=p.CategoryId

            });
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }
            product = await _unitOfWork.Products
    .Query()
    .Include(p => p.Category)
    .Include(p => p.Brand)
    .FirstOrDefaultAsync(p => p.Id == product.Id);
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Color = product.Color, //?? default(ProductColor) // Explicitly handle nullable ProductColor
                CategoryName=product.Category?.Name,
                BrandName=product.Brand?.Name,
                BrandId=product.BrandId,
                CategoryId=product.CategoryId

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
            product.BrandId = createProductDto.BrandId;
            product.CategoryId = createProductDto.CategoryId;
            product = await _unitOfWork.Products
    .Query()
    .Include(p => p.Category)
    .Include(p => p.Brand)
    .FirstOrDefaultAsync(p => p.Id == product.Id);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
