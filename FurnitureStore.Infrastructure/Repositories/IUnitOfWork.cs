using FurnitureStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Infrastructure.Repositories
{
    public interface IUnitOfWork: IDisposable
    {
       IGenericRepository<Product>Products { get; }
         IGenericRepository<Category>Categories { get; }
        IGenericRepository<Order>Orders { get; }
        IGenericRepository<OrderItem>OrderItems { get; }
        IGenericRepository<User>Users { get; }
        IGenericRepository<Brand> Brands { get; }
        IGenericRepository<CartItem> CartItems { get; }
        IGenericRepository<Address> Addresses { get; }
        IGenericRepository<RefreshToken> RefreshTokens { get; }
        Task<int> CompleteAsync();
    }
}
