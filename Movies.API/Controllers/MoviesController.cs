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
        return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie.ToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var movie = await movieRepo.GetByIdAsync(id);
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
