namespace Spotify.Shared.Models;

public class Song
{
    public string Id { get; init; }

    public string Name { get; init; }

    public string PreviewUrl { get; init; }

    public TimeSpan Duration { get; init; }

    public Album Album { get; init; }

    public IEnumerable<BaseArtist> Artists { get; init; }

    public string Uri { get; init; }

    public IEnumerable<string> ExternalUrls { get; init; }

    public int Popularity { get; init; }
}
