using Coin_up.Enums;

namespace Coin_up.Repositories
{
    public interface IQuestRepository : IRepository<Quest>
    {
        Task<IEnumerable<Quest>> GetActiveQuestsByUserIdAsync(Guid userId);
        Task<IEnumerable<Quest>> Get3ActiveQuestsByUserIdAsync(Guid userId);
        Task<List<Quest>> GetQuestsByStatusAsync(Guid userid, EnumQuestStatus status);
        Task DeleteByIdAsync(Guid id);

    }
}
