using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators;

public class MovieValidator : AbstractValidator<Movie>
{
    IMovieRepository _movieRepo;
    public MovieValidator(IMovieRepository movieRepo)
    {
        _movieRepo = movieRepo;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Movie Id cannot be empty");

        RuleFor(x => x.Genres)
            .NotEmpty()
            .WithMessage("Movie needs at least one genre");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Movie title cannot be empty");

        RuleFor(x => x.ReleaseYear)
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage("Cannot insert future movies");

        RuleFor(x => x.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("This movie already exists in the system");
    }

    private async Task<bool> ValidateSlug(Movie movie,
                                            string slug,
                                            CancellationToken token)
    {
        var existingMovie = await _movieRepo.GetBySlugAsync(slug, token);

        // if the slug exists and they have the same id it's an update so it's fine
        if (existingMovie is not null)
            return existingMovie.Id == movie.Id;

        return existingMovie is null;
    }
}
