using Coin_up.Dtos;

namespace Coin_up.Repositories
{
    public interface ITransacaoRepository : IRepository<Transacao>
    {
        Task<decimal> GetDespesaTotalAsync(Guid userId);
        Task<decimal> GetReceitaTotalAsync(Guid userId);
        Task<decimal> GetDespesaTotalComDataAsync(Guid userId, DateTime data);
        Task<decimal> GetReceitaTotalComDataAsync(Guid userId, DateTime data);
        Task<List<Transacao>> GetTransacaoListAsync(Guid userId);
    }
}
