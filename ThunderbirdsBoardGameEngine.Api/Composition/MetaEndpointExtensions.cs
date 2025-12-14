namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class MetaEndpointExtensions
    {
        public static IApplicationBuilder MapMetaEndpoint(this IApplicationBuilder app)
        {
            app.Map("/meta", builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new
                    {
                        serverVersion = "dev-0.0.0",    // TODO: Placeholder until CI wiring
                        catalogVersion = ApiVersionInfo.CatalogVersion,
                        name = "Thunderbirds Board Game Engine API"
                    };
                    await context.Response.WriteAsJsonAsync(response);
                });
            });

            return app;
        }
    }
}
