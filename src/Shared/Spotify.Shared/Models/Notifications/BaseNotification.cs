namespace Spotify.Shared.Models.Notifications;

public abstract class BaseNotification<T> where T : class
{
    public IEnumerable<T> Items { get; set; }

    public string DeviceId { get; set; }
}