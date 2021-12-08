namespace Spotify.Shared.Models;

public abstract class BaseNotification<T> where T : class
{
    public IEnumerable<T> Data { get; set; }
    public string DeviceId { get; set; }
}

public class ArtistNotification : BaseNotification<Artist>
{ }

public class AlbumNotification : BaseNotification<Album>
{ }

public class SongNotification : BaseNotification<Song>
{ }