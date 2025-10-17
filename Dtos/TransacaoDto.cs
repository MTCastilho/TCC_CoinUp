using Coin_up.Enums;

namespace Coin_up.Dtos
{
    public class TransacaoDto
    {
        public Guid Id { get; set; }
        public EnumTipoTransacao TipoTransacao { get; set; }
        public EnumCategoria Categoria { get; set; }
        public string? Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
    }
}
