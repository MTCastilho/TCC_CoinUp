using Coin_up.Enums;
using System.ComponentModel.DataAnnotations;

namespace Coin_up.Dtos
{
    public class CreateQuestInputDto
    {
        public string Descricao { get; set; }

        [Required]
        public EnumQuestObjectiveType TipoDeObjetivo { get; set; }

        // NOVO: Adicionado para o usuário escolher a duração
        [Required]
        public EnumQuestDuration Duracao { get; set; }

        public EnumCategoria? CategoriaAlvo { get; set; }

        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O Valor Alvo deve ser maior que zero.")]
        public decimal ValorAlvo { get; set; }
    }
}
