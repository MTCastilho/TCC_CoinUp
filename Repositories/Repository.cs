using Coin_up.Data;
using Microsoft.EntityFrameworkCore;

namespace Coin_up.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class   
    {
        protected readonly CoinUpDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(CoinUpDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
