using Coin_up.Enums;

namespace Coin_up.Dtos
{
    public class GerarQuestDto
    {
        public string Descricao { get; set; }
        public EnumQuestObjectiveType TipoDeObjetivo { get; set; }
        public EnumCategoria? CategoriaAlvo { get; set; }
        public decimal ValorAlvo { get; set; }
        public int PontosDeExperiencia { get; set; }
    }
}
