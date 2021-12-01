namespace Spotify.Shared.Models;

public class Album
{
    public string Id { get; init; }

    public string Name { get; init; }

    public string ReleaseDate { get; init; }

    public int TotalTracks { get; init; }

    public string Uri { get; init; }

    public IEnumerable<BaseArtist> Artists { get; init; }

    public IEnumerable<Image> Images { get; init; }
}
