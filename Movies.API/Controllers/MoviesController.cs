using Microsoft.AspNetCore.Mvc;
using Movies.API.Core;
using Movies.API.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.API.Controllers;

[ApiController]
public class MoviesController(IMovieService movieService) : ControllerBase
{
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get([FromRoute] string idOrSlug,
                                        CancellationToken cancellationToken)
    {
        var movie = Guid.TryParse(idOrSlug, out var id) ?
                await movieService.GetByIdAsync(id, cancellationToken) :
                await movieService.GetBySlugAsync(idOrSlug, cancellationToken);
        if (movie is null) return NotFound();

        return Ok(movie.ToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var movies = await movieService.GetAllAsync(cancellationToken);
        return Ok(movies.Select(m => m.ToResponse()));
    }
    
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request,
                                            CancellationToken cancellationToken)
    {
        var movie = request.ToModel();
        await movieService.CreateAsync(movie, cancellationToken);
        return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie.ToResponse());
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id,
                            [FromBody] UpdateMovieRequest request,
                            CancellationToken cancellationToken)
    {
        var movie = request.ToModel(id);
        if (await movieService.UpdateAsync(movie, cancellationToken) == null) return NotFound();

        return Ok(movie.ToResponse());
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id,
                                            CancellationToken cancellationToken)
    {
        var deleted = await movieService.DeleteByIdAsync(id, cancellationToken);
        if (!deleted) return NotFound();

        return Ok();   
    }
}
