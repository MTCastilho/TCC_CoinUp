using Coin_up.Dtos;
using Coin_up.Repositories;
using Coin_up.Services;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Coin_up.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/app/[controller]")]
    public class QuestController : ControllerBase
    {
        private readonly IQuestService _questService;
        private readonly IUsuarioRepository _usuarioRepository;

        public QuestController(IQuestService questService, IUsuarioRepository usuarioRepository)
        {
            _questService = questService;
            _usuarioRepository = usuarioRepository;
        }

        [HttpPost]
        [Route("criar-quest-ia")]
        public async Task<IActionResult> CreateAsync(string descricao)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _usuarioRepository.GetUsuarioIdByFirebaseUidAsync(firebaseUid);

            var quest = await _questService.CreatQuestAsync(userId, descricao);

            var response = new
            {
                StatusCode = 200,
                Mensage = "Quest criada com sucesso",
                Quest = quest
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("criar-quest-manual")]
        public async Task<IActionResult> CreateAsync(CreateQuestInputDto inputDto)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _usuarioRepository.GetUsuarioIdByFirebaseUidAsync(firebaseUid);

            var quest = await _questService.CreateQuestManualAsync(userId, inputDto);

            return Ok(new
            {
                StatusCode = 200,
                Mensage = "Quest criada com sucesso",
                Quest = quest
            });
        }
    }
}
