using Coin_up.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coin_up.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/app/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("login")]
        [AllowAnonymous]
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
