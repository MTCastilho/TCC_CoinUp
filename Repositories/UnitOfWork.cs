using Coin_up.Data;

namespace Coin_up.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoinUpDbContext _context;

        // As propriedades da interface são preenchidas com instâncias concretas dos repositórios
        public IUsuarioRepository Usuario { get; private set; }
        public IContaRepository Conta { get; private set; }
        public ITransacaoRepository Transacao { get; private set; }
        public IQuestRepository Quest { get; private set; }
        // public IContaRepository Contas { get; private set; } // Exemplo

        public UnitOfWork(CoinUpDbContext context)
        {
            _context = context;

            // Inicializa os repositórios, passando o MESMO DbContext para todos.
            // Isso é crucial para que todos trabalhem na mesma transação.
            Usuario = new UsuarioRepository(_context);
            Conta = new ContaRepository(_context);
            Transacao = new TransacaoRepository(_context);
            Quest = new QuestRepository(_context);
            // Contas = new ContaRepository(_context); // Exemplo
        }

        // <summary>
        // Comita a transação para o banco de dados.
        // </summary>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // <summary>
        // Libera a conexão do DbContext quando a Unit of Work for descartada.
        // </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
