using Coin_up.Data;
using Microsoft.EntityFrameworkCore;

namespace Coin_up.Repositories
{
    public class ContaRepository : Repository<Conta>, IContaRepository
    {
        private readonly CoinUpDbContext _dbContext;

        public ContaRepository(CoinUpDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Conta> GetContaByUserIdAsync(Guid userId)
        {
            return await _dbContext.Contas
                .Where(e => e.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> GetContaIdByUserIdAsync(Guid userId)
        {
            return await _dbContext.Contas
                .Where(c => c.UserId == userId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }
    }
}
