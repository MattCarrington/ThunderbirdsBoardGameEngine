using Microsoft.AspNetCore.StaticFiles;

namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class StaticFilesExtensions
    {
        public static IApplicationBuilder UseCustomStaticFiles(this IApplicationBuilder app)
        {
            // Configure content types – important for .dat ICU files
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings[".dat"] = "application/octet-stream";

            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = contentTypeProvider,
                // Belt-and-braces: if something still has an unknown extension, serve it as bytes
                ServeUnknownFileTypes = true,
                DefaultContentType = "application/octet-stream"
            });

            return app;
        }
    }
}
