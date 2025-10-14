using Coin_up.Dtos;

namespace Coin_up.Services
{
    public interface ITransacaoService
    {
        Task<bool> CreateTransacaoAsync(Guid userId, TransacaoInputDto input);

    }
}
