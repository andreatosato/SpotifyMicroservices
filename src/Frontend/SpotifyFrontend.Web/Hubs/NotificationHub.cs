using Microsoft.AspNetCore.SignalR;
using SpotifyFrontend.Web.Data;

namespace SpotifyFrontend.Web.Hubs;

public class NotificationHub : Hub
{
    private readonly ClientService clientService;

    public NotificationHub(ClientService clientService)
    {
        this.clientService = clientService;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        clientService.DeviceClient.Add(new DeviceClient(Context.ConnectionId));
        await Clients.Caller.SendAsync("UpdateDeviceId", Context.ConnectionId);
    }

    public void SetDeviceId(string deviceId)
    {
        var currentConnection = clientService.DeviceClient.Where(t => t.ConnectionId == Context.ConnectionId).First();
        currentConnection.DeviceId = deviceId;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var toRemove = clientService.DeviceClient.Where(t => t.ConnectionId == Context.ConnectionId).First();
        clientService.DeviceClient.Remove(toRemove);
        return base.OnDisconnectedAsync(exception);
    }
}
