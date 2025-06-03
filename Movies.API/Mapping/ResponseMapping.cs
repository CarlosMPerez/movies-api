using Movies.Application.Models;
using Movies.Contracts.Responses;

namespace Movies.API.Mapping;

public static class ResponseMapping
{
    public static MovieResponse ToResponse(this Movie movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            Slug = movie.Slug,
            ReleaseYear = movie.ReleaseYear,
            Genres = movie.Genres.ToList()
        };
    }

}
