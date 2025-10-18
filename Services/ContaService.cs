using AutoMapper;
using Coin_up.Dtos;
using Coin_up.Repositories;

namespace Coin_up.Services
{
    public class ContaService : IContaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IQuestRepository _questRepository;
        private readonly ITransacaoService _transacaoService;

        public ContaService(IUnitOfWork unitOfWork, IMapper mapper, IQuestRepository questRepository, ITransacaoService transacaoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _questRepository = questRepository;
            _transacaoService = transacaoService;
        }

        public async Task<ContaOutputDto> CreatContaAsync(Guid userId, ContaInputDto input)
        {
            var novaConta = new Conta
            {
                UserId = userId,
                Nome = input.Nome,
                SaldoAtual = input.SaldoAtual,
                TipoConta = input.TipoConta,
            };

            await _unitOfWork.Conta.AddAsync(novaConta);
            await _unitOfWork.CompleteAsync();

            return new ContaOutputDto
            {
                Id = novaConta.Id,
                Nome = novaConta.Nome,
                SaldoAtual = novaConta.SaldoAtual,
            };
        }

        public async Task<ContaOutputDto> UpdateContaAsync(Guid userId, ContaInputDto input)
        {
            var conta = await _unitOfWork.Conta.GetContaByUserIdAsync(userId);
            _mapper.Map(input, conta);

            await _unitOfWork.CompleteAsync();

            var output = _mapper.Map<Conta, ContaOutputDto>(conta);

            return output;
        }

        public async Task<ContaTransacaoDto> GetContaAndTransacoesAsync(Guid userId)
        {
            await _transacaoService.VerificaQuestAsync(userId);
            var conta = await _unitOfWork.Conta.GetContaByUserIdAsync(userId);
            var despesa = await _unitOfWork.Transacao.GetDespesaTotalAsync(userId);
            var receita = await _unitOfWork.Transacao.GetReceitaTotalAsync(userId);
            var quest = await _questRepository.Get3ActiveQuestsByUserIdAsync(userId);

            var contaOutput = _mapper.Map<Conta, ContaOutputDto>(conta);

            return new ContaTransacaoDto
            {
                Receita = receita,
                Despesa = despesa,
                Conta = contaOutput
            };
        }
    }
}
