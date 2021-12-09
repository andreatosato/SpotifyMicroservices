using Microsoft.AspNetCore.SignalR;
using Spotify.Shared;

namespace SpotifyFrontend.Web.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync(Constants.UpdateDeviceId, Context.ConnectionId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}
