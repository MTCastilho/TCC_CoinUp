using Coin_up.Dtos;
using Coin_up.Entities;
using Coin_up.Repositories;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Coin_up.Controllers
{
    [ApiController]
    [Route("api/app/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("login")]
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
