namespace Spotify.Shared.Models;

public class SearchRequest
{
    public string DeviceId { get; init; }

    public string AccessToken { get; init; }

    public string SearchText { get; init; }

    public SearchRequest(string deviceId, string accessToken, string searchText)
    {
        DeviceId = deviceId;
        AccessToken = accessToken;
        SearchText = searchText;
    }
}
