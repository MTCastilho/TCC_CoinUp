using Coin_up.Enums;

namespace Coin_up.Dtos
{
    public class TransacaoInputDto
    {
        public EnumTipoTransacao TipoTransacao { get; set; }
        public EnumCategoria Categoria { get; set; }
        public decimal Valor { get; set; }
    }
}
