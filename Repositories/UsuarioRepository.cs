using Coin_up.Data;
using Coin_up.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coin_up.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly CoinUpDbContext _dbContext;

        public UsuarioRepository(CoinUpDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> GetUsuarioByFirebaseUidAsync(string firebaseUid)
        {
            return await _dbContext.Usuarios.AnyAsync(a => a.FirebaseUid == firebaseUid);
        }

        public async Task<Guid> GetUsuarioIdByFirebaseUidAsync(string firebaseUid)
        {
            return await _dbContext.Usuarios
                .Where(u => u.FirebaseUid == firebaseUid)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario> GetUsuarioByUsuarioIdAsync(Guid id)
        {
            return await _dbContext.Usuarios
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
