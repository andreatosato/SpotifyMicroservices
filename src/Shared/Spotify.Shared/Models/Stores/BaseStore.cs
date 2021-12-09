namespace Spotify.Shared.Models;

public abstract class BaseStore<T> where T : class
{
    public IEnumerable<T> Items { get; set; }

    public string DeviceId { get; set; }

    public string SearchText { get; set; }
}
