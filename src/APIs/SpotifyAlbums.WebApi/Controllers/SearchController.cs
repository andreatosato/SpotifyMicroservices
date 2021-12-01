using System.Net.Mime;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace SpotifyAlbums.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class SearchController : ControllerBase
{
    [Topic("pubsub", "search")]
    [HttpPost]
    public async Task<IActionResult> Search(Spotify.Shared.Models.SearchRequest searchRequest)
    {
        var spotify = new SpotifyClient(searchRequest.AccessToken);
        var searchResults = await spotify.Search.Item(new(SearchRequest.Types.Album, searchRequest.SearchText));

        return NoContent();
    }
}
