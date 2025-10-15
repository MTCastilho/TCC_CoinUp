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
    public class ContaController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IContaService _contaService;
        public ContaController(IUsuarioRepository usuarioRepository, IContaService contaService)
        {
            _usuarioRepository = usuarioRepository;
            _contaService = contaService;
        }

        [HttpPost]
        [Route("criar-conta")]
        public async Task<IActionResult> CreatContaByUsuarioId(ContaInputDto input)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _usuarioRepository.GetUsuarioIdByFirebaseUidAsync(firebaseUid);
            var novaConta = await _contaService.CreatContaAsync(userId, input);

            return Ok(novaConta);
        }

        [HttpGet]
        [Route("tela-home")]
        public async Task<IActionResult> GetContaByUserId()
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _usuarioRepository.GetUsuarioIdByFirebaseUidAsync(firebaseUid);
            var tudo = await _contaService.GetContaAndTransacoesAsync(userId);

            return Ok(tudo);
        }

        [HttpPut]
        [Route("atualizar-conta")]
        public async Task<IActionResult> PutContaByUserId(ContaInputDto input)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _usuarioRepository.GetUsuarioIdByFirebaseUidAsync(firebaseUid);
            var resultado = await _contaService.UpdateContaAsync(userId, input);

            var responseParaCliente = new
            {
                StatusCode = 200,
                Message = "Dados atualizados com sucesso",
                Conta = resultado
            };

            return Ok(responseParaCliente);
        }
    }
}
