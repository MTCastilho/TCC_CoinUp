namespace Coin_up.Dtos
{
    public class HistoricoTransacaoDto
    {
        public DateTime Data { get; set; }
        public List<TransacaoDto> Transacoes { get; set; } = new List<TransacaoDto>();
    }
}
