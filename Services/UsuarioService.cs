using Coin_up.Dtos;
using Coin_up.Entities;
using Coin_up.Repositories;
using Microsoft.OpenApi.Extensions;

namespace Coin_up.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioOutputDto> CreateUsuarioAsync(UsuarioCadastroInputDto input)
        {
            var novoUsuario = new Usuario
            {
                FirebaseUid = input.FirebaseUid,
                Email = input.Email,
                Nome = input.Nome,
                Sexo = input.Sexo,
                Telefone = input.Telefone,
            };

            // ETAPA 3: Persistência (Salvar no banco)
            await _unitOfWork.Usuario.AddAsync(novoUsuario);
            await _unitOfWork.CompleteAsync();

            // ETAPA 4: Retorno (Mapear para um DTO de resposta)
            return new UsuarioOutputDto
            {
                Id = novoUsuario.Id,
                Nome = novoUsuario.Nome,
                Email = novoUsuario.Email,
                Rank = novoUsuario.Rank.GetDisplayName()
            };
        }
    }
}
