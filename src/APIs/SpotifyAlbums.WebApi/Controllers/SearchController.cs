using System.Net.Mime;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Spotify.Shared;
using Spotify.Shared.Models;
using Spotify.Shared.Models.Notifications;
using SpotifySearchRequest = SpotifyAPI.Web.SearchRequest;

namespace SpotifyAlbums.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class SearchController : ControllerBase
{
    private readonly DaprClient daprClient;

    public SearchController(DaprClient daprClient)
    {
        this.daprClient = daprClient;
    }

    [Topic(Constants.PubSubName, Constants.SearchTopic)]
    [HttpPost]
    public async Task<IActionResult> Search(SearchRequest searchRequest)
    {
        var spotify = new SpotifyAPI.Web.SpotifyClient(searchRequest.AccessToken);
        var searchResults = await spotify.Search.Item(new(SpotifySearchRequest.Types.Album, searchRequest.SearchText));

        var albums = searchResults.Albums.Items?.OrderBy(a => a.Name).Select(a => new Album
        {
            Id = a.Id,
            Name = a.Name,
            Uri = a.Uri,
            ReleaseDate = a.ReleaseDate,
            TotalTracks = a.TotalTracks,
            Artists = a.Artists.Select(artist => new BaseArtist
            {
                Id = artist.Id,
                Name = artist.Name,
                Uri = artist.Uri,
            }),
            Images = a.Images.Select(i => new Image
            {
                Url = i.Url,
                Height = i.Height,
                Width = i.Width
            })
        }).Take(3).ToList();

        if (albums != null)
        {
            await daprClient.PublishEventAsync(Constants.PubSubName, Constants.AlbumsSearched, new AlbumNotification
            {
                Items = albums,
                DeviceId = searchRequest.DeviceId
            });

            // Store the last 5 searches.
            var storeKey = $"latest-{searchRequest.DeviceId}";
            var latestSearches = await daprClient.GetStateAsync<List<AlbumStore>>(Constants.StoreName, storeKey) ?? new();

            var albumStore = new AlbumStore
            {
                Items = albums,
                DeviceId = searchRequest.DeviceId,
                SearchText = searchRequest.SearchText
            };

            latestSearches.Insert(0, albumStore);
            await daprClient.SaveStateAsync(Constants.StoreName, storeKey, latestSearches.Take(5));
        }

        return NoContent();
    }

    [HttpGet("latest/{deviceId}")]
    public async Task<IActionResult> GetLatestAsync([FromRoute] string deviceId)
    {
        var storeKey = $"latest-{deviceId}";
        var latestSearches = await daprClient.GetStateAsync<List<AlbumStore>>(Constants.StoreName, storeKey) ?? new();

        return Ok(latestSearches);
    }
}
