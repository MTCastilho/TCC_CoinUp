namespace Coin_up.Repositories
{
    public interface IQuestRepository : IRepository<Quest>
    {
        Task<IEnumerable<Quest>> GetActiveQuestsByUserIdAsync(Guid userId);
        Task<IEnumerable<Quest>> Get3ActiveQuestsByUserIdAsync(Guid userId);

    }
}
