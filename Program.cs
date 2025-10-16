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
    // Opcional: Adiciona informa��es b�sicas � sua documenta��o
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Minha API Protegida",
        Version = "v1",
        Description = "API para autentica��o e acesso a dados."
    });

    // 1. DEFINIR o esquema de seguran�a que a API usa (Bearer Token)
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Autentica��o JWT usando o esquema Bearer. \r\n\r\n " +
                      "Digite 'Bearer' [espa�o] e depois o seu token no campo abaixo. " +
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
        throw new ArgumentNullException(nameof(credentialPath), "O caminho para as credenciais do Firebase (FirebaseCredentialsPath) n�o foi encontrado na configura��o de desenvolvimento.");
    }

    // Carrega a credencial a partir do arquivo
    credential = GoogleCredential.FromFile(credentialPath);
}
else
{
    var firebaseConfigSection = builder.Configuration.GetSection("FirebaseCredentials");
    if (!firebaseConfigSection.Exists())
    {
        throw new InvalidOperationException("A configura��o 'FirebaseCredentials' n�o foi encontrada. Certifique-se de que a vari�vel de ambiente est� configurada corretamente em produ��o.");
    }

    // Converte a se��o de configura��o de volta para uma string JSON
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
