using AutoMapper;
using Coin_up.Dtos;

namespace Coin_up.Mappers
{
    public class ContaAutoMapper : Profile
    {
        public ContaAutoMapper()
        {
            CreateMap<ContaInputDto, Conta>();
            CreateMap<Conta, ContaOutputDto>();
        }
    }
}
