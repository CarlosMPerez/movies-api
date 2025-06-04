using System.Data;
using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    public async Task<bool> CreateAsync(Movie movie)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        // Dapper baby!
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into movies (id, slug, title, releaseyear)
            values (@Id, @Slug, @Title, @ReleaseYear)
        """, movie));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name)
                    values (@MovieId, @Name)
                """, new { MovieId = movie.Id, name = genre }));
            }
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
        """, new { id }));

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from movies where id = @id
        """, new { id }));

        transaction.Commit();

        return result > 0;
    }
    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var result = await connection.QueryAsync(new CommandDefinition("""
            select m.*, string_agg(g.name, ',') as moviegenres
            from movies as m 
            left join genres as g on m.id = g.movieid
            group by m.id
        """));

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            ReleaseYear = x.releaseyear,
            Genres = Enumerable.ToList(x.moviegenres.Split(','))
        });
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                select * from movies where id = @id
            """, new { id }));

        if (movie is null) return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                select name from genres where movieid = @id
            """, new { id })
        );

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                select * from movies where slug = @slug
            """, new { slug }));

        if (movie is null) return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                select name from genres where movieid = @id
            """, new { movie.Id })
        );

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<bool> UpdateAsync(Movie movie)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
        """, new { id = movie.Id }));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                insert into genres (movieId, name)
                values (@MovieId, @Name)
            """, new { MovieId = movie.Id, name = genre }));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            update movies 
            set Slug = @Slug, title = @Title, releaseyear = @ReleaseYear
            where id = @Id
        """, movie));

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select count(1) from movies where id = @id
        """, new { id }));
    }
}
