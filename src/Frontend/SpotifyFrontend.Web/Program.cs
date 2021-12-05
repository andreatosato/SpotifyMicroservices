using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR;
using Spotify.Shared.Models;
using SpotifyFrontend.Web.Data;
using SpotifyFrontend.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddDapr();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();
app.UseCloudEvents();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notification");
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapSubscribeHandler();
    endpoints.MapPost("albums", async (IEnumerable<Album> albums, IHubContext<NotificationHub> notificationHub) =>
    {
        await notificationHub.Clients.All.SendAsync("albumNews", albums);
    })
    .WithTopic("pubsub", "AlbumsResearched");
});

app.Run();

