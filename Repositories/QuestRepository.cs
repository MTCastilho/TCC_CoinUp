using Coin_up.Data;
using Coin_up.Enums;
using Microsoft.EntityFrameworkCore;

namespace Coin_up.Repositories
{
    public class QuestRepository : Repository<Quest>, IQuestRepository
    {
        private readonly CoinUpDbContext _dbContext;
        public QuestRepository(CoinUpDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Quest>> GetActiveQuestsByUserIdAsync(Guid userId)
        {
            return await _dbContext.Quests
                .Where(q => q.UserId == userId && q.Status == EnumQuestStatus.Ativa)
                .ToListAsync();

        }

        public async Task<IEnumerable<Quest>> Get3ActiveQuestsByUserIdAsync(Guid userId)
        {
            return await _dbContext.Quests
                .Where(q => q.UserId == userId)
                .Where(q => q.Status == EnumQuestStatus.Ativa)
                .OrderBy(q => q.DataDeCriacao)
                .Take(4).ToListAsync();
        }

        public async Task<List<Quest>> GetQuestsByStatusAsync(Guid userid, EnumQuestStatus status)
        {
            EnumQuestStatus statusInfo;

            if(status == null)
            {
                return await _dbContext.Quests
                    .Where(q => q.UserId == userid)
                    .Where(q => q.Status == EnumQuestStatus.Ativa)
                    .OrderBy(q => q.DataDeCriacao)
                    .ToListAsync();
            }

            return await _dbContext.Quests
                    .Where(q => q.UserId == userid)
                    .Where(q => q.Status == status)
                    .OrderBy(q => q.DataDeCriacao)
                    .ToListAsync();
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var questToDelete = await _dbContext.Quests
                .Where(q => q.Id == id)
                .FirstOrDefaultAsync();

            _dbContext.Quests.Remove(questToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}
