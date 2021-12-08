namespace Spotify.Shared.Models;

public abstract class BaseStore<T> where T : class
{
    public IEnumerable<T> Data { get; set; }
    public string DeviceId { get; set; }
    public string SearchText { get; set; }
}

public class ArtistStore : BaseStore<Artist>
{ }

public class AlbumStore : BaseStore<Album>
{ }

public class SongStore : BaseStore<Song>
{ }

public class StoreResult
{
    public List<ArtistStore> ArtistsStore { get; set; }
    public List<SongStore> SongsStore { get; set; }
    public List<AlbumStore> AlbumsStore { get; set; }
}