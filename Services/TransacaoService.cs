using Coin_up.Dtos;
using Coin_up.Entities;
using Coin_up.Enums;
using Coin_up.Repositories;

namespace Coin_up.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransacaoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateTransacaoAsync(Guid userId, TransacaoInputDto input)
        {
            var contaId = await _unitOfWork.Conta.GetContaIdByUserIdAsync(userId);
            var conta = await _unitOfWork.Conta.GetContaByUserIdAsync(userId);
            var usuario = await _unitOfWork.Usuario.GetUsuarioByUsuarioIdAsync(userId);

            var novaTransacao = new Transacao
            {
                UsuarioId = userId,
                ContaId = contaId,
                TipoTransacao = input.TipoTransacao,
                Categoria = input.Categoria,
                Valor = input.Valor,
                Descricao = input.Descricao
            };

            switch (input.TipoTransacao)
            {
                case EnumTipoTransacao.Despesa:
                    if (conta.SaldoAtual < input.Valor)
                    {
                        // Lançar uma exceção específica para o controller tratar como 400 Bad Request
                        throw new InvalidOperationException("Saldo insuficiente para realizar a transação.");
                    }
                    conta.SaldoAtual -= input.Valor;
                    break;

                case EnumTipoTransacao.Receita:
                    conta.SaldoAtual += input.Valor;
                    break;
            }

            await _unitOfWork.Transacao.AddAsync(novaTransacao);
            await AtualizarQuestsAposTransacao(novaTransacao, userId, conta, usuario);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private async Task AtualizarQuestsAposTransacao(Transacao novaTransacao, Guid userId, Conta conta, Usuario usuario)
        {

            // Busca todas as quests ATIVAS do usuário no banco
            var questsAtivas = await _unitOfWork.Quest.GetActiveQuestsByUserIdAsync(novaTransacao.UsuarioId);

            // Se não houver quests ativas, não há nada a fazer.
            if (!questsAtivas.Any()) return;

            foreach (var quest in questsAtivas)
            {
                // Verifica se a quest já expirou
                if (quest.DataDeExpiracao.HasValue && quest.DataDeExpiracao.Value < DateTime.UtcNow)
                {
                    quest.Status = EnumQuestStatus.Expirada;
                    continue; // Pula para a próxima quest
                }

                bool questFoiConcluida = false;
                    
                // Aplica a lógica de acordo com o tipo de objetivo da quest
                switch (quest.TipoDeObjetivo)
                {
                    // "Gaste no máximo R$ 200 na categoria Alimentação"
                    case EnumQuestObjectiveType.LimitarGastoEmCategoria:
                        if (novaTransacao.TipoTransacao == EnumTipoTransacao.Despesa && quest.CategoriaAlvo == novaTransacao.Categoria)
                        {
                            // Busca o total já gasto na categoria desde que a quest começou
                            var totalGasto = await _unitOfWork.Transacao.GetDespesaTotalComDataAsync(userId, quest.DataDeCriacao);

                            // Atualiza o progresso (quanto do limite já foi gasto)
                            quest.ProgressoAtual = (int)((totalGasto / quest.ValorAlvo) * 100);

                            // Se o total gasto ultrapassou o alvo, a quest falhou
                            if (totalGasto > quest.ValorAlvo)
                            {
                                quest.Status = EnumQuestStatus.Falhou;
                            }
                        }

                        if (quest.DataDeExpiracao.HasValue)
                        {
                            // Calcula a duração total da missão em dias.
                            var duracaoTotal = (quest.DataDeExpiracao.Value - quest.DataDeCriacao).TotalDays;

                            if (duracaoTotal > 0)
                            {
                                // Calcula quantos dias se passaram desde o início.
                                var duracaoDecorrida = (DateTime.UtcNow - quest.DataDeCriacao).TotalDays;

                                // O progresso é a porcentagem do tempo que o usuário "sobreviveu".
                                quest.ProgressoAtual = (int)Math.Clamp((duracaoDecorrida / duracaoTotal) * 100, 0, 100);
                            }
                        }
                        break;

                    // "Não tenha nenhuma despesa na categoria Lazer por 7 dias"
                    case EnumQuestObjectiveType.NaoGastarEmCategoria:
                        if (novaTransacao.TipoTransacao == EnumTipoTransacao.Despesa && quest.CategoriaAlvo == novaTransacao.Categoria)
                        {
                            // Se o usuário gastou na categoria proibida, a quest falhou instantaneamente
                            quest.Status = EnumQuestStatus.Falhou;
                        }
                        break;

                    // "Economize R$ 100 no total do mês"
                    case EnumQuestObjectiveType.EconomizarValorTotal:
                        // Recalcula a economia total (Receitas - Despesas) desde o início da quest
                        decimal totalReceitas = await _unitOfWork.Transacao.GetReceitaTotalComDataAsync(userId, quest.DataDeCriacao);
                        decimal totalDespesas = await _unitOfWork.Transacao.GetDespesaTotalComDataAsync(userId, quest.DataDeCriacao);
                        decimal economiaAtual = totalReceitas - totalDespesas;

                        // Atualiza o progresso com base na meta de economia
                        if (quest.ValorAlvo > 0)
                        {
                            quest.ProgressoAtual = (int)((economiaAtual / quest.ValorAlvo) * 100);
                        }
                        break;

                    // "Mantenha o saldo da conta X acima de R$ 500"
                    case EnumQuestObjectiveType.ManterSaldoAcimaDe:
                        // Se o saldo da conta ficou abaixo do alvo, a quest falhou
                        if (conta.SaldoAtual < quest.ValorAlvo)
                        {
                            quest.Status = EnumQuestStatus.Falhou;
                        }
                        break;
                }

                // Após atualizar o progresso, verifica se a quest foi concluída
                if (quest.ProgressoAtual >= 100 && quest.Status == EnumQuestStatus.Ativa)
                {
                    quest.Status = EnumQuestStatus.Concluida;
                    quest.DataDeConclusao = DateTime.UtcNow;

                    // Adiciona a experiência ao usuário!
                    usuario.PontosDeExperiencia += quest.PontosDeExperiencia;
                }
            }

            // Após processar todas as quests, verifica se o usuário subiu de nível
            VerificarLevelUpUsuario(usuario);
        }

        // Em seu TransacaoService.cs, adicione este método privado
        private void VerificarLevelUpUsuario(Usuario usuario)
        {
            // Supondo que você tenha a classe estática GerenciadorDeNivel que criamos antes
            int xpParaProximoNivel = CalcularXpParaProximoNivel(usuario.Nivel); // Supondo que 'Nivel' seja uma prop em Usuario

            while (usuario.PontosDeExperiencia >= xpParaProximoNivel)
            {
                // Sobe de nível
                usuario.Nivel++;
                // Subtrai o XP usado
                usuario.PontosDeExperiencia -= xpParaProximoNivel;

                // Recalcula o XP para o *próximo* nível
                xpParaProximoNivel = CalcularXpParaProximoNivel(usuario.Nivel);
            }
        }

        private const double XP_BASE = 100;
        private const double FATOR_DE_PROGRESSAO = 1.5;

        public static int CalcularXpParaProximoNivel(int nivelAtual)
        {
            if (nivelAtual <= 0)
            {
                return (int)XP_BASE;
            }

            double xpNecessario = XP_BASE * Math.Pow(nivelAtual, FATOR_DE_PROGRESSAO);

            return (int)Math.Floor(xpNecessario);
        }
    }
}
