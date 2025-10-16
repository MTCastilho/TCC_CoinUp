using Coin_up.Dtos;
using Coin_up.Enums;
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
    public class QuestController : ControllerBase
    {
        private readonly IQuestService _questService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuestController(IQuestService questService, IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
        {
            _questService = questService;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("criar-quest-ia")]
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

        [HttpPost("criar-quest-manual")]
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

        [HttpGet("buscar-quests")]
        public async Task<IActionResult> GetListAsync(EnumQuestStatus status)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = await _usuarioRepository.GetUsuarioIdByFirebaseUidAsync(firebaseUid);
            var list = await _unitOfWork.Quest.GetQuestsByStatusAsync(userId, status);

            return Ok(list);
        }
    }
}
