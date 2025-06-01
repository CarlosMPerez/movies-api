using Microsoft.AspNetCore.Mvc;
using Movies.API.Core;
using Movies.API.Mapping;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movies.API.Controllers;

[ApiController]
public class MoviesController(IMovieRepository movieRepo) : ControllerBase
{

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var movie = request.ToModel();
        await movieRepo.CreateAsync(movie);
        return Created($"/{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
    }
}
