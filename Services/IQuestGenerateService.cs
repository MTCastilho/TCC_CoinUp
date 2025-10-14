using Coin_up.Dtos;

namespace Coin_up.Services
{
    public interface IQuestGenerateService
    {
        Task<GeneratedQuestData> InterpretObjectiveAsync(string objetivoUsuario);
    }
}
