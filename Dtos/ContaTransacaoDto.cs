namespace Coin_up.Dtos
{
    public class ContaTransacaoDto
    {
        public decimal Receita { get; set; }
        public decimal Despesa { get; set; }
        public ContaOutputDto Conta { get; set; }
    }
}
