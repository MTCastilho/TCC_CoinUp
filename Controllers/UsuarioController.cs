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
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("cadastro")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAsync([FromBody] UsuarioCadastroInputDto input)
        {
            var usuario = await _usuarioService.CreateUsuarioAsync(input);

            return Ok(usuario);
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
