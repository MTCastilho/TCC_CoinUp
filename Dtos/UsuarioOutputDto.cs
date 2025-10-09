using Coin_up.Enums;

namespace Coin_up.Dtos
{
    public class UsuarioOutputDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Rank { get; set; }
    }
}
