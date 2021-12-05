namespace SpotifyFrontend.Web.Data;

public class ClientService
{
    public List<DeviceClient> DeviceClient { get; set; } = new List<DeviceClient>();
}
public class DeviceClient
{
    public DeviceClient(string connectionId)
    {
        ConnectionId = connectionId;
    }

    public string ConnectionId { get; init; }
    public string DeviceId { get; set; }
}