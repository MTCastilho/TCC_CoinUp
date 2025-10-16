using Coin_up.Dtos;
using Coin_up.Entities;
using Coin_up.Repositories;
using Coin_up.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coin_up.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/app/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioController(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost("cadastro")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync([FromBody] UsuarioCadastroInputDto input)
        {
            try
            {
                var userArgs = new UserRecordArgs { Email = input.Email, Password = input.Senha, EmailVerified = true, Disabled = false };
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);

                var novoUsuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Email = input.Email,
                    Nome = input.Nome,
                    Telefone = input.Telefone,
                    Sexo = input.Sexo,
                    FirebaseUid = userRecord.Uid
                };

                var novaConta = new Conta
                {
                    UserId = novoUsuario.Id,
                };

                await _unitOfWork.Usuario.AddAsync(novoUsuario);
                await _unitOfWork.Conta.AddAsync(novaConta);
                await _unitOfWork.CompleteAsync();

                return Ok(new
                {
                    Code = StatusCodes.Status200OK,
                    Message = "Usuario cadastrado com sucesso",
                    Usuario = novoUsuario
                });
            }
            catch (FirebaseAuthException e)
            {
                // 4. Se o Firebase retornou um erro específico, trata-o aqui
                string friendlyMessage;
                switch (e.AuthErrorCode)
                {
                    case AuthErrorCode.EmailAlreadyExists:
                        friendlyMessage = "Este endereço de e-mail já está em uso por outra conta.";
                        // 409 Conflict é o status HTTP mais apropriado para este caso
                        return Conflict(new { Message = friendlyMessage });

                    default:
                        friendlyMessage = "Ocorreu um erro inesperado durante o cadastro no Firebase.";
                        // Retorna um erro genérico para outros problemas do Firebase
                        return BadRequest(new { Message = friendlyMessage, FirebaseError = e.Message });
                }
            }
            catch (Exception ex)
            {
                // 5. Para qualquer outro tipo de erro (ex: problema de rede)
                return StatusCode(500, new { Message = "Ocorreu um erro interno no servidor.", Error = ex.Message });
            }
        }

        [HttpGet("buscar-usuario")]
        public async Task<IActionResult> GetUsuarioPorFirebaseUid()
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _usuarioRepository.GetUsuarioIdByFirebaseUidAsync(firebaseUid);
            var usuario = await _unitOfWork.Usuario.GetByIdAsync(userId);

            return Ok(usuario);
        }
    }
}
