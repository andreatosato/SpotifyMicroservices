using System.Net.Mime;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Spotify.Shared;
using Spotify.Shared.Models;
using Spotify.Shared.Models.Notifications;
using SpotifySearchRequest = SpotifyAPI.Web.SearchRequest;

namespace SpotifySongs.WebApi.Controllers;

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
        }).Take(3).ToList();

        if (songs != null)
        {
            await daprClient.PublishEventAsync(Constants.PubSubName, Constants.SongsSearched, new SongNotification
            {
                Items = songs,
                DeviceId = searchRequest.DeviceId
            });

            // Store the last 5 searches.
            var storeKey = $"latest-{searchRequest.DeviceId}";
            var latestSearches = await daprClient.GetStateAsync<List<SongStore>>(Constants.StoreName, storeKey) ?? new();

            var songStore = new SongStore
            {
                Items = songs,
                DeviceId = searchRequest.DeviceId,
                SearchText = searchRequest.SearchText
            };

            latestSearches.Insert(0, songStore);
            await daprClient.SaveStateAsync(Constants.StoreName, storeKey, latestSearches.Take(5));
        }

        return NoContent();
    }

    [HttpGet("latest/{deviceId}")]
    public async Task<IActionResult> GetLatestAsync([FromRoute] string deviceId)
    {
        var storeKey = $"latest-{deviceId}";
        var latestSearches = await daprClient.GetStateAsync<List<SongStore>>(Constants.StoreName, storeKey) ?? new List<SongStore>();

        return Ok(latestSearches);
    }
}
