using System.Net.Mime;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Spotify.Shared.Models;
using SpotifySearchRequest = SpotifyAPI.Web.SearchRequest;

namespace SpotifyArtists.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class SearchController : ControllerBase
{
    [Topic("pubsub", "search")]
    [HttpPost]
    public async Task<IActionResult> Search(SearchRequest searchRequest)
    {
        var spotify = new SpotifyAPI.Web.SpotifyClient(searchRequest.AccessToken);
        var searchResults = await spotify.Search.Item(new(SpotifySearchRequest.Types.Artist, searchRequest.SearchText));

        var artists = searchResults.Artists.Items?.OrderBy(a => a.Name).Select(a => new Artist
        {
            Id = a.Id,
            Name = a.Name,
            Uri = a.Uri,
            Followers = a.Followers.Total,
            Genres = a.Genres,
            Popularity = a.Popularity,
            Images = a.Images.Select(i => new Image
            {
                Url = i.Url,
                Height = i.Height,
                Width = i.Width
            })
        });

        return NoContent();
    }
}
