using System.Net.Mime;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpotifyBackend.Settings;

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
    public IActionResult Get(string deviceId, [FromQuery(Name = "q")] string searchText)
    {
        return Accepted();
    }
}
