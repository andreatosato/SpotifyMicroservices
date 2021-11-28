using Dapr.Client;
using Dapr.Extensions.Configuration;
using SpotifyBackend.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddDaprSecretStore("appsecrets", new DaprClientBuilder().Build(), new[] { ":" });

// Add services to the container.
builder.Services.AddControllers().AddDapr();

builder.Services.Configure<SpotifySettings>(builder.Configuration.GetSection(nameof(SpotifySettings)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Spotify Backend API v1");
});

app.MapControllers();

app.Run();
