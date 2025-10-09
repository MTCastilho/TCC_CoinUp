using Coin_up.Enums;

namespace Coin_up.Dtos
{
    public class UsuarioCadastroInputDto
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public EnumSexo Sexo { get; set; }
    }
}
