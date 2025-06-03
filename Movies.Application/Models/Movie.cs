
using System.Net;
using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }
    public string Slug => GenerateSlug();
    public required string Title { get; set; }

    public required int ReleaseYear { get; set; }
    public required List<string> Genres { get; init; } = new();

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .Replace(" ", "-")
            .ToLowerInvariant();
        return $"{sluggedTitle}-{ReleaseYear}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}
