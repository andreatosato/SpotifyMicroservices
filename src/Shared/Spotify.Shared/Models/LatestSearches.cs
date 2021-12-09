namespace Spotify.Shared.Models;

public class LatestSearches
{
    public IEnumerable<ArtistStore> ArtistsStore { get; set; }

    public IEnumerable<SongStore> SongsStore { get; set; }

    public IEnumerable<AlbumStore> AlbumsStore { get; set; }
}