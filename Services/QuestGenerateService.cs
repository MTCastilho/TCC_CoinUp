using Coin_up.Dtos;
using Coin_up.Enums;
using System.Text.Json;

namespace Coin_up.Services
{
    public class QuestGenerateService : IQuestGenerateService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _geminiApiUrl;
        private readonly ILogger<string> _logger;

        public QuestGenerateService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<string> logger)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = _configuration["GoogleAI:ApiKey"];
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new ArgumentNullException(nameof(_apiKey), "A ApiKey do Google AI não foi encontrada na configuração.");
            }

            _geminiApiUrl = $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={_apiKey}";
            _logger = logger;
        }

        public class GeminiRequest
        {
            public Content[] Contents { get; set; }
        }

        public class Content
        {
            public Part[] Parts { get; set; }
        }

        public class Part
        {
            public string Text { get; set; }
        }

        public class GeminiResponse
        {
            public Candidate[] Candidates { get; set; }
        }

        public class Candidate
        {
            public Content Content { get; set; }
        }

        public async Task<GeneratedQuestData> InterpretObjectiveAsync(string objetivoUsuario)
        {
            string prompt = BuildPrompt(objetivoUsuario);
            string jsonResponse = await CallGeminiApi(prompt);

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                return null; // Retorna nulo se a API não gerar conteúdo
            }

            try
            {
                // Melhoria 2: Bloco try-catch para desserialização segura
                string cleanedJsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();

                var interpretedData = JsonSerializer.Deserialize<GeneratedQuestData>(cleanedJsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return interpretedData;
            }
            catch (JsonException ex)
            {
                // Loga o erro e o JSON inválido para depuração
                _logger.LogError(ex, "Falha ao desserializar a resposta JSON da API Gemini. Resposta recebida: {jsonResponse}", jsonResponse);
                // Você pode optar por retornar null ou lançar uma exceção customizada
                return null;
            }
        }

        private async Task<string> CallGeminiApi(string prompt)
        {
            var requestPayload = new GeminiRequest
            {
                Contents = new[]
                {
                    new Content { Parts = new[] { new Part { Text = prompt } } }
                }
            };

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_geminiApiUrl, requestPayload);

                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro na API do Gemini. Status: {StatusCode}. Detalhes: {ErrorContent}", response.StatusCode, errorContent);
                    return null; // Retornar nulo em caso de erro de API
                }

                var responseData = await response.Content.ReadFromJsonAsync<GeminiResponse>();

                string generatedText = responseData?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                return generatedText;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao chamar a API do Gemini.");
                return null;
            }
        }

        private string BuildPrompt(string objetivoDoUsuario)
        {
            string tiposDeQuestPermitidos = string.Join(", ", Enum.GetNames(typeof(EnumQuestObjectiveType)));
            string tiposCategoriasPermitidas = string.Join(", ", Enum.GetNames(typeof(EnumCategoria)));

            return $@"
                Você é uma IA designer de missões para o aplicativo de finanças 'CoinUp'.
                Sua tarefa é analisar o perfil financeiro de um usuário e criar uma missão (quest) personalizada e motivadora para ele.

                **Estes são os únicos tipos de missão que você pode criar:**
                {tiposDeQuestPermitidos}
                **Exemplo**
                **Regra Principal: A missão criada DEVE se encaixar em um dos seguintes tipos de objetivo.
                É importante lembrar que essas categorias tem um valor especifico que corresponde a elas no meu codigo, indo de 0 a 3.
                Estes são os únicos tipos permitidos no sistema:**
                - **EconomizarValorTotal(0)**: Use quando o usuário quer que a diferença entre receitas e despesas atinja um valor. Ex: ""quero juntar R$ 500"".
                - **LimitarGastoEmCategoria(1)**: Use quando o usuário quer gastar MENOS ou um valor MÁXIMO em algo específico. Ex: ""gastar menos com delivery"", ""não passar de R$ 200 em lazer"".
                - **NaoGastarEmCategoria(2)**: Use quando o usuário quer ZERAR os gastos em uma categoria por um tempo. Ex: ""parar de comprar jogos"", ""ficar um mês sem fast-food"".
                - **ManterSaldoAcimaDe(3)**: Use quando o objetivo é sobre manter um saldo MÍNIMO. Ex: ""não deixar minha conta ficar abaixo de R$ 100"".

                **Estas são as categorias de missão que voce pode usar:**
                {tiposCategoriasPermitidas}
                **É importante lembrar que essas categorias tem um valor especifico que corresponde a elas no meu codigo, indo de 0 a 6.**
                Deverá ser usado APENAS os valores correspondentes
                Outros = 0,
                Salario = 1,
                Moradia = 2,
                Alimentacao = 3,
                Transporte = 4,
                Saude = 5,
                Lazer = 6,

                **Esta é a descrição do que o usuario quer atingir com essa quest:**
                {objetivoDoUsuario}

                **Suas instruções são:**
                1.  **Analise o perfil:** Identifique uma área onde o usuário pode melhorar (ex: gastos altos em uma categoria, poucas receitas, etc.).
                2.  **Escolha o tipo de objetivo:** Com base na sua análise, escolha o 'tipoDeObjetivo' mais apropriado da lista de tipos permitidos.
                3.  **Defina os parâmetros:** Defina um 'valorAlvo' realista e uma 'categoriaAlvo' (se aplicável) com base nos dados do perfil.
                4.  **Gere o conteúdo criativo:** Crie a 'descricao', 'pontosDeExperiencia' e 'duracaoEmDias'.

                Responda APENAS com um objeto JSON válido. O JSON deve conter TODAS as seguintes chaves:
                - ""tipoDeObjetivo"": O valor exato de um dos tipos permitidos. Ex: ""tipoDeObjetivo"":""1"" pois 1 é o equivalente a LimitarGastoEmCategoria como passado antes
                - ""valorAlvo"": Um número decimal.
                - ""categoriaAlvo"": O numero de uma das categorias. Ex: ""categoriaAlvo"":""3"" pois 3 é o equivalente a aliemtanção como passado antes
                - ""descricao"": O texto da missão para o usuário.
                - ""pontosDeExperiencia"": Um número inteiro.
                - ""duracaoEmDias"": Um número inteiro.

                Exemplo de Resposta Esperada:
                {{
                    ""tipoDeObjetivo"": ""1"",
                    ""valorAlvo"": 450.50,
                    ""categoriaAlvo"": ""3"",
                    ""descricao"": ""Manter gastos de alimentação abaixo de R$ 450,50 este mês"",
                    ""pontosDeExperiencia"": 200,
                    ""duracaoEmDias"": 30
                }}
            ";
        }
    }
}
