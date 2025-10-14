using AutoMapper;
using Coin_up.Dtos;
using Coin_up.Enums;
using Coin_up.Repositories;

namespace Coin_up.Services
{
    public class QuestService : IQuestService
    {
        private readonly IQuestGenerateService _questGenerateService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        

        public QuestService(IQuestGenerateService questGenerateService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _questGenerateService = questGenerateService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<QuestOutputDto> CreatQuestAsync(Guid userId, string objetivo)
        {
            var dadosGeradosPelaIA = await _questGenerateService.InterpretObjectiveAsync(objetivo);

            var novaQuest = new Quest
            {
                UserId = userId,
                Descricao = dadosGeradosPelaIA.Descricao,
                DataDeExpiracao = DateTime.Now.AddDays(dadosGeradosPelaIA.DuracaoEmDias),
                TipoDeObjetivo = (EnumQuestObjectiveType)dadosGeradosPelaIA.TipoDeObjetivo,
                CategoriaAlvo = (EnumCategoria)dadosGeradosPelaIA.CategoriaAlvo,
                PontosDeExperiencia = dadosGeradosPelaIA.PontosDeExperiencia,
                ValorAlvo = dadosGeradosPelaIA.ValorAlvo
            };

            await _unitOfWork.Quest.AddAsync(novaQuest);
            await _unitOfWork.CompleteAsync();

            var output = _mapper.Map<Quest, QuestOutputDto>(novaQuest);

            return output;
        }

        public async Task<Quest> CreateQuestManualAsync(Guid userId, CreateQuestInputDto questDto)
        {
            var dataDeExpiracao = CalcularDataDeExpiracao(questDto.Duracao);
            var pontosDeExperiencia = CalcularPontosDeExperiencia(questDto.TipoDeObjetivo, questDto.ValorAlvo, questDto.Duracao);

            var novaQuest = new Quest
            {
                UserId = userId,
                Descricao = questDto.Descricao,
                TipoDeObjetivo = questDto.TipoDeObjetivo,
                CategoriaAlvo = questDto.CategoriaAlvo,
                ValorAlvo = questDto.ValorAlvo,
                DataDeExpiracao = dataDeExpiracao,
                PontosDeExperiencia = pontosDeExperiencia,
            };

            await _unitOfWork.Quest.AddAsync(novaQuest);
            await _unitOfWork.CompleteAsync();

            return novaQuest;
        }

        private DateTime CalcularDataDeExpiracao(EnumQuestDuration duracao)
        {
            var agora = DateTime.UtcNow;
            switch (duracao)
            {
                case EnumQuestDuration.Diaria:
                    var fimDoDia = new DateTime(agora.Year, agora.Month, agora.Day, 23, 59, 59, DateTimeKind.Utc);
                    return fimDoDia;

                case EnumQuestDuration.Semanal:
                    return agora.AddDays(7);

                case EnumQuestDuration.Mensal:
                    var ultimoDia = DateTime.DaysInMonth(agora.Year, agora.Month);
                    var fimDoMes = new DateTime(agora.Year, agora.Month, ultimoDia, 23, 59, 59, DateTimeKind.Utc);
                    return fimDoMes;

                default:
                    throw new ArgumentOutOfRangeException(nameof(duracao), "Duração da quest inválida.");
            }
        }

        private int CalcularPontosDeExperiencia(EnumQuestObjectiveType tipoDeObjetivo, decimal valorAlvo, EnumQuestDuration duracao)
        {
            int xpBase;
            decimal fatorDificuldade = 1.0m;

            switch (tipoDeObjetivo)
            {
                case EnumQuestObjectiveType.EconomizarValorTotal:
                    xpBase = 150;
                    fatorDificuldade += (valorAlvo / 500m); // Mais a economizar = mais difícil
                    break;
                case EnumQuestObjectiveType.LimitarGastoEmCategoria:
                    xpBase = 100;
                    if (valorAlvo > 0) fatorDificuldade += (1000m / valorAlvo); // Menor o limite = mais difícil
                    break;
                case EnumQuestObjectiveType.NaoGastarEmCategoria:
                    xpBase = 200;
                    fatorDificuldade = 1.5m; // Dificuldade binária (conseguiu ou não)
                    break;
                case EnumQuestObjectiveType.ManterSaldoAcimaDe:
                    xpBase = 120;
                    fatorDificuldade += (valorAlvo / 1000m); // Maior o saldo a manter = mais difícil
                    break;
                default:
                    xpBase = 50; // Um valor padrão
                    break;
            }

            decimal xpInicial = (decimal)xpBase * fatorDificuldade;

            decimal multiplicadorDeDuracao;
            switch (duracao)
            {
                case EnumQuestDuration.Diaria:
                    multiplicadorDeDuracao = 1.0m;
                    break;
                case EnumQuestDuration.Semanal:
                    multiplicadorDeDuracao = 1.8m;
                    break;
                case EnumQuestDuration.Mensal:
                    multiplicadorDeDuracao = 3.5m;
                    break;
                default:
                    multiplicadorDeDuracao = 1.0m;
                    break;
            }

            int xpFinal = (int)Math.Round(xpInicial * multiplicadorDeDuracao);

            return Math.Max(xpFinal, 50); // Garante que toda quest dê no mínimo 50 XP
        }
        
    }
}
