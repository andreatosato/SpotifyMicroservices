namespace Spotify.Shared.Models;

public class Artist : BaseArtist
{
    public int Followers { get; init; }

    public IEnumerable<string> Genres { get; init; }

    public int Popularity { get; init; }

    public IEnumerable<Image> Images { get; init; }
}
