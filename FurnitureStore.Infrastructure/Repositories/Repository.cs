using FurnitureStore.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FurnitureStore.Infrastructure.Repositories
{
    public class Repository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task AddAsync(T entity)=> await _context.Set<T>().AddAsync(entity);


        public async Task AddRangeAsync(IEnumerable<T> entities)=> await _context.Set<T>().AddRangeAsync(entities);

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public  void DeleteRange(IEnumerable<T> entities)=> _context.Set<T>().RemoveRange(entities);

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)=>await _context.Set<T>().Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync()=>await _context.Set<T>().ToListAsync();


        public async Task<T?> GetByIdAsync(int id)
        {
         return await  _context.Set<T>().FindAsync(id);
        }
        public IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }
        //overload with includes
        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
