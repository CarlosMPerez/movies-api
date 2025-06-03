using System;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = new();
    public Task<bool> CreateAsync(Movie movie)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        return Task.FromResult(_movies.RemoveAll(x => x.Id == id) > 0);
    }

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_movies.SingleOrDefault(x => x.Id == id));
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return Task.FromResult(_movies.SingleOrDefault(x => x.Slug == slug));
    }

    public Task<bool> UpdateAsync(Movie movie)
    {
        var ndx = _movies.FindIndex(x => x.Id == movie.Id);
        if (ndx < 0)
        {
            return Task.FromResult(false);
        }

        _movies[ndx] = movie;
        return Task.FromResult(true);
    }
}
