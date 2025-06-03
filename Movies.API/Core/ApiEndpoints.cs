namespace Movies.API.Core;

public static class ApiEndpoints
{
    private const string ApiPrefix = "api";

    public static class Movies
    {
        private const string BasePath = $"{ApiPrefix}/movies";
        public const string Create = BasePath;
        public const string Get = $"{BasePath}/{{idOrSlug}}";
        public const string GetAll = BasePath;
        public const string Update = $"{BasePath}/{{id:guid}}";
        public const string Delete = $"{BasePath}/{{id:guid}}";
    }
}
