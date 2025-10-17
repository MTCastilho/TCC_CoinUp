using Coin_up.Data;
using Coin_up.Dtos;
using Coin_up.Enums;
using Microsoft.EntityFrameworkCore;

namespace Coin_up.Repositories
{
    public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
    {
        private readonly CoinUpDbContext _dbContext;

        public TransacaoRepository(CoinUpDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<decimal> GetDespesaTotalAsync(Guid userId)
        {
            var total = await _dbContext.Transacoes
                .Where(e => e.UsuarioId == userId)
                .Where(e => e.TipoTransacao == EnumTipoTransacao.Despesa)
                .SumAsync(e => (decimal?)e.Valor);

            return total ?? 0;
        }

        public async Task<decimal> GetReceitaTotalAsync(Guid userId)
        {
            var total = await _dbContext.Transacoes
                .Where(e => e.UsuarioId == userId)
                .Where(e => e.TipoTransacao == EnumTipoTransacao.Receita)
                .SumAsync(e => (decimal?)e.Valor);

            return total ?? 0;
        }

        public async Task<decimal> GetDespesaTotalComDataAsync(Guid userId, DateTime data)
        {
            var total = await _dbContext.Transacoes
                .Where(e => e.UsuarioId == userId)
                .Where(e => e.TipoTransacao == EnumTipoTransacao.Despesa)
                .Where(e => e.Data == data)
                .SumAsync(e => (decimal?)e.Valor);

            return total ?? 0;
        }

        public async Task<decimal> GetReceitaTotalComDataAsync(Guid userId, DateTime data)
        {
            var total = await _dbContext.Transacoes
                .Where(e => e.UsuarioId == userId)
                .Where(e => e.TipoTransacao == EnumTipoTransacao.Receita)
                .Where(e => e.Data == data)
                .SumAsync(e => (decimal?)e.Valor);

            return total ?? 0;
        }

        public async Task<List<HistoricoTransacaoDto>> GetTransacaoListAsync(Guid userId)
        {
            return await _dbContext.Transacoes
                .Where(e => e.UsuarioId == userId)
                .OrderByDescending(a => a.Data)
                .GroupBy(t => t.Data.Date)
                .Select(g => new HistoricoTransacaoDto
                {
                    Data = g.Key,
                    Transacoes = g.Select(t => new TransacaoDto
                    {
                        Id = t.Id,
                        TipoTransacao = t.TipoTransacao,
                        Categoria = t.Categoria,
                        Descricao = t.Descricao,
                        Valor = t.Valor,
                        Data = t.Data
                    })
                    .ToList()
                })
                .ToListAsync();
        }
    }
}
