using FurnitureStore.Domain.Entities;
using FurnitureStore.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        public IGenericRepository<Product> Products { get; }

        public IGenericRepository<Category> Categories { get; }

        public IGenericRepository<Order> Orders { get; }

        public IGenericRepository<OrderItem> OrderItems { get; }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<Brand> Brands { get; }

        public IGenericRepository<CartItem> CartItems{ get; }

        public IGenericRepository<Address> Addresses { get; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Products = new Repository<Product>(_context);
            Categories = new Repository<Category>(_context);
            Brands = new Repository<Brand>(_context);
            Users = new Repository<User>(_context);
            Orders = new Repository<Order>(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
