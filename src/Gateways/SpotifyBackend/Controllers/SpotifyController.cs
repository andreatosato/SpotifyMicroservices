using System.Net.Mime;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Spotify.Shared;
using Spotify.Shared.Models;
using SpotifyAPI.Web;
using SpotifyBackend.Settings;
using SearchRequest = Spotify.Shared.Models.SearchRequest;

namespace SpotifyBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class SpotifyController : ControllerBase
{
    private readonly SpotifySettings spotifySettings;
    private readonly DaprClient daprClient;

    public SpotifyController(IOptions<SpotifySettings> spotifySettingsOptions, DaprClient daprClient)
    {
        spotifySettings = spotifySettingsOptions.Value;
        this.daprClient = daprClient;
    }

    [HttpGet("{deviceId}/search")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Get(string deviceId, [FromQuery(Name = "q"), BindRequired] string searchText)
    {
        var config = SpotifyClientConfig.CreateDefault();

        var request = new ClientCredentialsRequest(spotifySettings.ClientId!, spotifySettings.ClientSecret!);
        var response = await new OAuthClient(config).RequestToken(request);

        var searchRequest = new SearchRequest(deviceId, response.AccessToken, searchText);
        await daprClient.PublishEventAsync(Constants.PubSubName, Constants.SearchTopic, searchRequest);

        return Accepted();
    }

    [HttpGet("{deviceId}/latest")]
    [ProducesResponseType(typeof(LatestSearches), StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> GetLatest(string deviceId)
    {
        var latestAlbumsTask = daprClient.InvokeMethodAsync<IEnumerable<AlbumStore>>(HttpMethod.Get, "albums", $"api/search/latest/{deviceId}");
        var latestSongsTask = daprClient.InvokeMethodAsync<IEnumerable<SongStore>>(HttpMethod.Get, "songs", $"api/search/latest/{deviceId}");
        var latestsArtistsTask = daprClient.InvokeMethodAsync<IEnumerable<ArtistStore>>(HttpMethod.Get, "artists", $"api/search/latest/{deviceId}");

        await Task.WhenAll(latestAlbumsTask, latestSongsTask, latestsArtistsTask);

        return Ok(new LatestSearches
        {
            AlbumsStore = await latestAlbumsTask,
            SongsStore = await latestSongsTask,
            ArtistsStore = await latestsArtistsTask
        });
    }
}
