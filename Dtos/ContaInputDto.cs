using Coin_up.Enums;

namespace Coin_up.Dtos
{
    public class ContaInputDto
    {
        public string Nome { get; set; }
        public decimal SaldoAtual { get; set; }
        public EnumTipoConta TipoConta { get; set; }
    }
}
