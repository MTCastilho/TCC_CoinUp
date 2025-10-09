using Coin_up.Data;


namespace Coin_up.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CoinUpDbContext _context;

        // As propriedades da interface são preenchidas com instâncias concretas dos repositórios
        public IUsuarioRepository Usuarios { get; private set; }
        // public IContaRepository Contas { get; private set; } // Exemplo

        public UnitOfWork(CoinUpDbContext context)
        {
            _context = context;

            // Inicializa os repositórios, passando o MESMO DbContext para todos.
            // Isso é crucial para que todos trabalhem na mesma transação.
            Usuarios = new UsuarioRepository(_context);
            // Contas = new ContaRepository(_context); // Exemplo
        }

        /// <summary>
        /// Este é o único lugar no seu código onde o SaveChangesAsync será chamado.
        /// Ele comita a transação para o banco de dados.
        /// </summary>
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Libera a conexão do DbContext quando a Unit of Work for descartada.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
