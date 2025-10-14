using Coin_up.Dtos;
using Coin_up.Repositories;
using Coin_up.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coin_up.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/app/[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITransacaoService _transacaoService;

        public TransacaoController(IUsuarioRepository usuarioRepository, ITransacaoService transacaoService)
        {
            _usuarioRepository = usuarioRepository;
            _transacaoService = transacaoService;
        }

        [HttpPost]
        [Route("criar-transacao")]
        public async Task<IActionResult> CreateAsync(TransacaoInputDto input)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _usuarioRepository.GetUsuarioIdByFirebaseUidAsync(firebaseUid);
            var transacao = await _transacaoService.CreateTransacaoAsync(userId, input);

            if (transacao)
            {
                return Ok("Transação realizada com sucesso");
            }

            return BadRequest("Houve um problema durante a comunicação, tente novamente mais tearde.");
        }
    }
}
