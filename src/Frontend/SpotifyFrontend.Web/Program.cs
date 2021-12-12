using Microsoft.AspNetCore.SignalR;
using MudBlazor.Services;
using Spotify.Shared;
using Spotify.Shared.Models.Notifications;
using SpotifyFrontend.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages().AddDapr();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

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

    endpoints.MapPost("albums", async (AlbumNotification albumNotification, IHubContext<NotificationHub> notificationHub) =>
    {
        await notificationHub.Clients.Client(albumNotification.DeviceId).SendAsync(Constants.AlbumsAvailable, albumNotification.Items);
    })
    .WithTopic(Constants.PubSubName, Constants.AlbumsSearched);

    endpoints.MapPost("songs", async (SongNotification songNotification, IHubContext<NotificationHub> notificationHub) =>
    {
        await notificationHub.Clients.Client(songNotification.DeviceId).SendAsync(Constants.SongsAvailable, songNotification.Items);
    })
    .WithTopic(Constants.PubSubName, Constants.SongsSearched);

    endpoints.MapPost("artists", async (ArtistNotification artistNotification, IHubContext<NotificationHub> notificationHub) =>
    {
        await notificationHub.Clients.Client(artistNotification.DeviceId).SendAsync(Constants.ArtistsAvailable, artistNotification.Items);
    })
    .WithTopic(Constants.PubSubName, Constants.ArtistsSearched);
});

app.Run();

