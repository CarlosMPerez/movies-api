using Movies.Application.Models;
using Movies.Contracts.Requests;

namespace Movies.API.Mapping;

public static class ContractMapping
{
    // Contract --> Application
    public static Movie ToModel(this CreateMovieRequest request)
    {
        return new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            ReleaseYear = request.ReleaseYear,
            Genres = request.Genres.ToList()
        };
    }
    public static Movie ToModel(this UpdateMovieRequest request, Guid id)
    {
        return new Movie
        {
            Id = id,
            Title = request.Title,
            ReleaseYear = request.ReleaseYear,
            Genres = request.Genres.ToList()
        };
    }
}
