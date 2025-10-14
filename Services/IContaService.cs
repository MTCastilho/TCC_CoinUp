using Coin_up.Dtos;

namespace Coin_up.Services
{
    public interface IContaService
    {
        Task<ContaOutputDto> CreatContaAsync(Guid userId, ContaInputDto input);

        Task<ContaOutputDto> UpdateContaAsync(Guid userId, ContaInputDto input);

        Task<ContaTransacaoDto> GetContaAndTransacoesAsync(Guid userId);

    }
}
