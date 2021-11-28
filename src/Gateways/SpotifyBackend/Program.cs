using Dapr.Client;
using Dapr.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddDapr();

//TODO: SECRETS
builder.Configuration.AddDaprSecretStore("demosecrets", new DaprClientBuilder().Build(), new[] { ":" });
var stringSecret = builder.Configuration["super-secret"];
//TODO: SECRETS


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
