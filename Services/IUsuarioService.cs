using Coin_up.Dtos;

namespace Coin_up.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioOutputDto> CreateUsuarioAsync(string firebaseUid, string email, UsuarioCadastroInputDto input);
    }
}
