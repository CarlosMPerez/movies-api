using System;

namespace Movies.API.Core;

public static class ApiEndpoints
{
    private const string ApiPrefix = "api";

    public static class Movies
    {
        private const string BasePath = $"{ApiPrefix}/movies";
        public const string Create = BasePath;
    }
}
