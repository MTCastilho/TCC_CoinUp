using Coin_up.Dtos;
using Coin_up.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coin_up.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("/cadastro")]
        public async Task<IActionResult> CreateAsync([FromBody] UsuarioCadastroInputDto input)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            var usuario = await _usuarioService.CreateUsuarioAsync(firebaseUid, email, input);

            return Ok(usuario);
        }
    }
}
