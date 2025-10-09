using System.ComponentModel.DataAnnotations;

namespace Coin_up.Enums
{
    public enum EnumRank
    {
        [Display(Name = "Nenhum")]
        Nenhum = 0,

        [Display(Name = "Cobre")]
        Cobre = 1,

        [Display(Name = "Bronze")]
        Bronze = 2,

        [Display(Name = "Prata")]
        Prata = 3,

        [Display(Name = "Ouro")]
        Ouro = 4,

        [Display(Name = "Platina")]
        Platina = 5,

        [Display(Name = "Diamante")]
        Diamante = 6,

        [Display(Name = "Lendário")]
        Lendario = 7
    }
}
