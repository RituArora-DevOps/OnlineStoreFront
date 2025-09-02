using ECommerceSecureApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly OnlineStoreDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(OnlineStoreDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(object id) => await _dbSet.FindAsync(id);

        public async Task InsertAsync(T entity) => await _dbSet.AddAsync(entity);

        public async Task UpdateAsync(T entity) => _dbSet.Update(entity);

        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null) _dbSet.Remove(entity);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
