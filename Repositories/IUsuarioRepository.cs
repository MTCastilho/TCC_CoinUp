using Coin_up.Dtos;
using Coin_up.Entities;

namespace Coin_up.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<bool> GetUsuarioByFirebaseUidAsync(string firebaseUid);

        Task<Guid> GetUsuarioIdByFirebaseUidAsync(string firebaseUid);
        Task<Usuario> GetUsuarioByUsuarioIdAsync(Guid id);

    }
}
