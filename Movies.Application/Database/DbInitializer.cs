using System;
using Dapper;

namespace Movies.Application.Database;

public class DbInitializer(IDbConnectionFactory dbConnectionFactory)
{
    public async Task InitializeDatabaseAsync()
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("""
            create table if not exists movies (
                id UUID primary key,
                slug TEXT not null,
                title TEXT not null,
                releaseyear integer not null);
        """);
        await connection.ExecuteAsync("""
            create unique index concurrently if not exists movies_slug_idx 
            on movies
            using btree(slug);
        """);
    }
}
