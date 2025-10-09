namespace Coin_up.Enums
{
    public enum EnumQuestObjectiveType
    {
        // Ex: Economize R$ 100 no total do mês
        EconomizarValorTotal = 0,
    
        // Ex: Gaste no máximo R$ 200 na categoria Alimentação
        LimitarGastoEmCategoria = 1,
    
        // Ex: Não tenha nenhuma despesa na categoria Lazer por 7 dias
        NaoGastarEmCategoria = 2,
    
        // Ex: Cadastre 10 despesas este mês
        RegistrarNumeroDeTransacoes = 3,

        // Ex: Mantenha o saldo da conta X acima de R$ 500
        ManterSaldoAcimaDe = 4
    }
}
