using Coin_up.Dtos;

namespace Coin_up.Services
{
    public interface IQuestService
    {
        Task<QuestOutputDto> CreatQuestAsync(Guid userId, string objetivo);
        Task<Quest> CreateQuestManualAsync(Guid userId, CreateQuestInputDto questDto);


    }
}
