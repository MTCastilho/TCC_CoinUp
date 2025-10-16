using Coin_up.Data;
using Coin_up.Repositories;
using Coin_up.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

string firebaseProjectId = builder.Configuration["FireBase:LocalId"];
string connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
GoogleCredential credential;

// Add services to the container
builder.Services.AddDbContext<CoinUpDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddControllers();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<IQuestGenerateService, QuestGenerateService>();
builder.Services.AddScoped<IQuestService, QuestService>();
builder.Services.AddScoped<IQuestRepository, QuestRepository>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true,
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Opcional: Adiciona informações básicas à sua documentação
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Minha API Protegida",
        Version = "v1",
        Description = "API para autenticação e acesso a dados."
    });

    // 1. DEFINIR o esquema de segurança que a API usa (Bearer Token)
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Autenticação JWT usando o esquema Bearer. \r\n\r\n " +
                      "Digite 'Bearer' [espaço] e depois o seu token no campo abaixo. " +
                      "\r\n\r\nExemplo: Bearer 12345abcdef"
    });

    // 2. EXIGIR que o Swagger UI envie o token nas chamadas para os endpoints
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

if (builder.Environment.IsDevelopment())
{
    var credentialPath = builder.Configuration["FireBase:FirebaseCredentialsPath"];
    if (string.IsNullOrEmpty(credentialPath))
    {
        throw new ArgumentNullException(nameof(credentialPath), "O caminho para as credenciais do Firebase (FirebaseCredentialsPath) não foi encontrado na configuração de desenvolvimento.");
    }

    // Carrega a credencial a partir do arquivo
    credential = GoogleCredential.FromFile(credentialPath);
}
else
{
    var firebaseConfigSection = builder.Configuration.GetSection("FirebaseCredentials");
    if (!firebaseConfigSection.Exists())
    {
        throw new InvalidOperationException("A configuração 'FirebaseCredentials' não foi encontrada. Certifique-se de que a variável de ambiente está configurada corretamente em produção.");
    }

    // Converte a seção de configuração de volta para uma string JSON
    var firebaseConfigJson = JsonSerializer.Serialize(firebaseConfigSection.Get<object>());

    // Carrega a credencial a partir da string JSON
    credential = GoogleCredential.FromJson(firebaseConfigJson);
}

FirebaseApp.Create(new AppOptions()
{
    Credential = credential,
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
