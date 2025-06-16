using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository, IValidator<Movie> validator) : IMovieService
{
    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return movieRepository.GetAllAsync(cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id,
                                CancellationToken cancellationToken = default)
    {
        return movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(string slug,
                                CancellationToken cancellationToken = default)
    {
        return movieRepository.GetBySlugAsync(slug, cancellationToken);
    }

    public async Task<bool> CreateAsync(Movie movie,
                                CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(movie, cancellationToken: cancellationToken);   
        return await movieRepository.CreateAsync(movie, cancellationToken);
    }

    public Task<bool> DeleteByIdAsync(Guid id,
                                CancellationToken cancellationToken = default)
    {
        return movieRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie,
                                CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(movie, cancellationToken: cancellationToken);   
        if (!await movieRepository.ExistsByIdAsync(movie.Id, cancellationToken)) return null;

        await movieRepository.UpdateAsync(movie, cancellationToken);
        return movie;
    }
}
