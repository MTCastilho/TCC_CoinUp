using Coin_up.Enums;
using System.ComponentModel.DataAnnotations;

namespace Coin_up.Dtos
{
    public class TransacaoInputDto
    {
        public EnumTipoTransacao TipoTransacao { get; set; }
        public EnumCategoria Categoria { get; set; }
        public decimal Valor { get; set; }

        [MaxLength(50)]
        public string? Descricao { get; set; }
        public DateTime Data { get; set; }
    }
}
