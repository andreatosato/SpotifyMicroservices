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
builder.Services.AddSingleton<ClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();
app.UseCloudEvents();
app.UseRouting();
var clientService = app.Services.GetService<ClientService>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notification");
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapSubscribeHandler();
    endpoints.MapPost("albums", async (AlbumNotification albumNotification, IHubContext<NotificationHub> notificationHub) =>
    {
        var deviceIdConnection = clientService.DeviceClient.Where(t => t.DeviceId == albumNotification.DeviceId).First();
        //await notificationHub.Clients.Client(deviceIdConnection.ConnectionId).SendAsync("albumNews", albumNotification.Data);
        await notificationHub.Clients.All.SendAsync("albumNews", albumNotification.Data);
    })
    .WithTopic("pubsub", "AlbumsResearched");

    endpoints.MapPost("songs", async (SongNotification songNotification, IHubContext<NotificationHub> notificationHub) =>
    {
        var deviceIdConnection = clientService.DeviceClient.Where(t => t.DeviceId == songNotification.DeviceId).First();
        //await notificationHub.Clients.Client(deviceIdConnection.ConnectionId).SendAsync("songsNews", songNotification.Data);
        await notificationHub.Clients.All.SendAsync("songsNews", songNotification.Data);
    })
    .WithTopic("pubsub", "SongResearched");

    endpoints.MapPost("artists", async (ArtistNotification artistNotification, IHubContext<NotificationHub> notificationHub) =>
    {
        var deviceIdConnection = clientService.DeviceClient.Where(t => t.DeviceId == artistNotification.DeviceId).First();
        //await notificationHub.Clients.Client(deviceIdConnection.ConnectionId).SendAsync("artistsNews", artistNotification.Data);
        await notificationHub.Clients.All.SendAsync("artistsNews", artistNotification.Data);
    })
    .WithTopic("pubsub", "ArtistResearched");

});

app.Run();

