using Coin_up.Enums;

namespace Coin_up.Dtos
{
    public class QuestOutputDto
    {
        public string Descricao { get; set; }
        public DateTime DataDeExpiracao { get; set; }
        public EnumQuestObjectiveType TipoDeObjetivo { get; set; }
        public EnumCategoria CategoriaAlvo { get; set; }
        public int PontosDeExperiencia { get; set; }
        public decimal ValorAlvo { get; set; }
    }
}
