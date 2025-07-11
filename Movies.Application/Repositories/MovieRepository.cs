using System.Data;
using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository(IDbConnectionFactory dbConnectionFactory) : IMovieRepository
{
    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var result = await connection.QueryAsync(new CommandDefinition("""
            select m.*, string_agg(g.name, ',') as moviegenres
            from movies as m 
            left join genres as g on m.id = g.movieid
            group by m.id
        """, cancellationToken: cancellationToken));

        return result.Select(x => new Movie
        {
            Id = x.id,
            Title = x.title,
            ReleaseYear = x.releaseyear,
            Genres = Enumerable.ToList(x.moviegenres.Split(','))
        });
    }

    public async Task<Movie?> GetByIdAsync(Guid id,
                                CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                select * from movies where id = @id
            """, new { id }, cancellationToken: cancellationToken));

        if (movie is null) return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                select name from genres where movieid = @id
            """, new { id }, cancellationToken: cancellationToken)
        );

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug,
                                CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition("""
                select * from movies where slug = @slug
            """, new { slug }, cancellationToken: cancellationToken));

        if (movie is null) return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
                select name from genres where movieid = @id
            """, new { movie.Id }, cancellationToken: cancellationToken)
        );

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<bool> CreateAsync(Movie movie,
                                CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        // Dapper baby!
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            insert into movies (id, slug, title, releaseyear)
            values (@Id, @Slug, @Title, @ReleaseYear)
        """, movie, cancellationToken: cancellationToken));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    insert into genres (movieId, name)
                    values (@MovieId, @Name)
                """, new { MovieId = movie.Id, name = genre },
                cancellationToken: cancellationToken));
            }
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id,
                                CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
        """, new { id }, cancellationToken: cancellationToken));

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            delete from movies where id = @id
        """, new { id }, cancellationToken: cancellationToken));

        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> UpdateAsync(Movie movie,
                                CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            delete from genres where movieid = @id
        """, new { id = movie.Id }, cancellationToken: cancellationToken));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                insert into genres (movieId, name)
                values (@MovieId, @Name)
            """, new { MovieId = movie.Id, name = genre },
            cancellationToken: cancellationToken));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            update movies 
            set Slug = @Slug, title = @Title, releaseyear = @ReleaseYear
            where id = @Id
        """, movie, cancellationToken: cancellationToken));

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id,
                                CancellationToken cancellationToken = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
            select count(1) from movies where id = @id
        """, new { id }, cancellationToken: cancellationToken));
    }
}
