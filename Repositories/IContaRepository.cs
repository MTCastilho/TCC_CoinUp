using Coin_up.Dtos;

namespace Coin_up.Repositories
{
    public interface IContaRepository : IRepository<Conta>
    {
        Task<Conta> GetContaByUserIdAsync(Guid userId);

        Task<Guid> GetContaIdByUserIdAsync(Guid userId);
    }
}
