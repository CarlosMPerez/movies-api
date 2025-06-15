using System;
using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Validators;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository, IValidator<Movie> validator) : IMovieService
{

    public Task<IEnumerable<Movie>> GetAllAsync()
    {
        return movieRepository.GetAllAsync();
    }

    public Task<Movie?> GetByIdAsync(Guid id)
    {
        return movieRepository.GetByIdAsync(id);
    }

    public Task<Movie?> GetBySlugAsync(string slug)
    {
        return movieRepository.GetBySlugAsync(slug);
    }

    public async Task<bool> CreateAsync(Movie movie)
    {
        await validator.ValidateAndThrowAsync(movie);   
        return await movieRepository.CreateAsync(movie);
    }

    public Task<bool> DeleteByIdAsync(Guid id)
    {
        return movieRepository.DeleteByIdAsync(id);
    }

    public async Task<Movie?> UpdateAsync(Movie movie)
    {
        await validator.ValidateAndThrowAsync(movie);   
        if (!await movieRepository.ExistsByIdAsync(movie.Id)) return null;

        await movieRepository.UpdateAsync(movie);
        return movie;
    }
}
