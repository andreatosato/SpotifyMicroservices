using System.Net.Mime;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Spotify.Shared.Models;
using SpotifySearchRequest = SpotifyAPI.Web.SearchRequest;

namespace SpotifySongs.WebApi.Controllers;

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
        var searchResults = await spotify.Search.Item(new(SpotifySearchRequest.Types.Track, searchRequest.SearchText));

        var songs = searchResults.Tracks.Items?.OrderBy(t => t.Name).Select(t => new Song
        {
            Id = t.Id,
            Name = t.Name,
            Uri = t.Uri,
            PreviewUrl = t.PreviewUrl,
            Duration = TimeSpan.FromMilliseconds(t.DurationMs),
            ExternalUrls = t.ExternalUrls?.Select(u => u.Value),
            Popularity = t.Popularity,
            Artists = t.Artists.Select(a => new BaseArtist
            {
                Id = a.Id,
                Name = a.Name,
                Uri = a.Uri,
            }),
            Album = t.Album != null ? new Album
            {
                Id = t.Album.Id,
                Name = t.Album.Name,
                Uri = t.Album.Uri,
                ReleaseDate = t.Album.ReleaseDate,
                TotalTracks = t.Album.TotalTracks,
                Artists = t.Album.Artists.Select(a => new BaseArtist
                {
                    Id = a.Id,
                    Name = a.Name,
                    Uri = a.Uri,
                }),
                Images = t.Album.Images.Select(i => new Image
                {
                    Url = i.Url,
                    Height = i.Height,
                    Width = i.Width
                })
            } : null
        });

        return NoContent();
    }
}
