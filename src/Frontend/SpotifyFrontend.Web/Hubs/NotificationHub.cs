using Microsoft.AspNetCore.SignalR;

namespace SpotifyFrontend.Web.Hubs;

public class NotificationHub : Hub
{
    public NotificationHub()
    {
    }
    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}
