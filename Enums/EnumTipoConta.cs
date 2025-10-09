using System.ComponentModel.DataAnnotations;

namespace Coin_up.Enums
{
    public enum EnumTipoConta
    {
        Outros = 0,
        Carteira = 1,
        ContaCorrente = 2,
        Poupanca = 3,
        Investimentos = 4,
        [Display(Name = "Vale Alimentação/Vale Refeição")]
        Vr_Vl = 5,
    }
}
