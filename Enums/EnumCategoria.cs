using System.ComponentModel.DataAnnotations;

namespace Coin_up.Enums
{
    public enum EnumCategoria
    {
        [Display(Name = "Outros")]
        Outros = 0,

        // --- RECEITAS ---
        [Display(Name = "Salário")]
        Salario = 1,

        // --- DESPESAS ESSENCIAIS ---
        [Display(Name = "Moradia")]
        Moradia = 2,

        [Display(Name = "Alimentação")]
        Alimentacao = 3,

        [Display(Name = "Transporte")]
        Transporte = 4,

        [Display(Name = "Saúde")]
        Saude = 5,

        // --- DESPESAS PESSOAIS E LAZER ---
        [Display(Name = "Lazer e Hobbies")]
        Lazer = 6,
    }
}
