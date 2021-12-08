using System.Net.Mime;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Spotify.Shared.Models;
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

    [Topic("pubsub", "search")]
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

        if(albums!= null)
            await daprClient.PublishEventAsync("pubsub", "AlbumsResearched", new AlbumNotification { Data = albums, DeviceId = searchRequest.DeviceId });

        // Conservo le ultime 5 ricerche
        var latestStore = await daprClient.GetStateAsync<List<AlbumStore>>("state-managment", $"latest-{searchRequest.DeviceId}") ?? new();
        var albumStore = new AlbumStore()
        {
            Data = albums,
            DeviceId = searchRequest.DeviceId,
            SearchText = searchRequest.SearchText
        };
        latestStore.Insert(0, albumStore);
        await daprClient.SaveStateAsync("state-managment", $"latest-{searchRequest.DeviceId}", latestStore.Take(5));

        return NoContent();
    }


    [HttpGet]
    public async Task<IActionResult> GetLatestAsync(string deviceId)
    {
        var latestStore = await daprClient.GetStateAsync<List<AlbumStore>>("state-managment", $"latest-{deviceId}");
        return Ok(latestStore);
    }
}
