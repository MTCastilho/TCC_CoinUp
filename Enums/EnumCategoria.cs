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

        [Display(Name = "Pix")]
        Pix = 2,

        [Display(Name = "Cheque")]
        Cheque = 8,

        [Display(Name = "Dinheiro")]
        Dinheiro = 9,

        // --- DESPESAS ESSENCIAIS ---
        [Display(Name = "Moradia")]
        Moradia = 3,

        [Display(Name = "Alimentação")]
        Alimentacao = 4,

        [Display(Name = "Transporte")]
        Transporte = 5,

        [Display(Name = "Saúde")]
        Saude = 6,

        // --- DESPESAS PESSOAIS E LAZER ---
        [Display(Name = "Lazer e Hobbies")]
        Lazer = 7,

        
    }
}
