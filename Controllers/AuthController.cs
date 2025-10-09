using Coin_up.Dtos;
using Coin_up.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;

namespace Coin_up.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<string> _logger;

        public AuthController(IUsuarioRepository usuarioRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<string> logger)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/login")]
        public async Task<IActionResult> GetUsuarioFirebaseUidAsync(string email, string password)
        {
            var firebaseApiKey = _configuration["FireBase:ApiKey"];

            var googleApiUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={firebaseApiKey}";

            var requestBody = new
            {
                email,
                password,
                returnSecureToken = true
            };

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync(googleApiUrl, requestBody);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<FirebaseAuthResponseDto>();
                var responseParaCliente = new
                {
                    StatusCode = 200,
                    Message = "Usuário logado com sucesso!",
                    Token = authResponse.IdToken
                };
                return Ok(responseParaCliente);
            }
            else
            {
                return BadRequest("Usuario ou senha incorretos");
            }
        }
    }
}
