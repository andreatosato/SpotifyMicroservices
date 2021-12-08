using Microsoft.AspNetCore.SignalR;

namespace SpotifyFrontend.Web.Hubs;

public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("UpdateDeviceId", Context.ConnectionId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}
