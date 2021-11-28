using System.Net.Mime;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
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
        await daprClient.PublishEventAsync("pubsub", "search", searchRequest);

        return Accepted();
    }
}
