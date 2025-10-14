using AutoMapper;
using Coin_up.Dtos;

namespace Coin_up.Mappers
{
    public class QuestAutoMapper : Profile
    {
        public QuestAutoMapper()
        {
            CreateMap<Quest, QuestOutputDto>();
        }
    }
}
