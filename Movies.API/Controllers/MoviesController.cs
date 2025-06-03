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
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie.ToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug)
    {
        var movie = Guid.TryParse(idOrSlug, out var id) ?
                await movieRepo.GetByIdAsync(id) :
                await movieRepo.GetBySlugAsync(idOrSlug);
        if (movie is null) return NotFound();

        return Ok(movie.ToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll()
    {
        var movies = await movieRepo.GetAllAsync();
        return Ok(movies.Select(m => m.ToResponse()));
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id,
                            [FromBody] UpdateMovieRequest request)
    {
        var movie = request.ToModel(id);
        var updated = await movieRepo.UpdateAsync(movie);
        if (!updated) return NotFound();

        return Ok(movie.ToResponse());
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var deleted = await movieRepo.DeleteByIdAsync(id);
        if (!deleted) return NotFound();

        return Ok();   
    }
}
