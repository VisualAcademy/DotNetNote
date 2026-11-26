using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace DotNetNote.Documentation;

/// <summary>Serves generated public DocFX pages without involving application authentication.</summary>
internal static class DocFxDocumentation
{
    public static void UseDotNetNoteDocs(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var localRoot = Path.Combine(env.ContentRootPath, "obj", "DocFxSite");
        var publishedRoot = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "docs");
        var root = File.Exists(Path.Combine(localRoot, "index.html"))
            ? localRoot
            : publishedRoot;

        if (!File.Exists(Path.Combine(root, "index.html")))
        {
            // The docs build may have been deliberately disabled.
            return;
        }

        var provider = new PhysicalFileProvider(root);
        app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>()
            .ApplicationStopped.Register(provider.Dispose);

        // Map only /docs and its descendants, before MVC/Blazor routing.
        app.Map("/docs", docs =>
        {
            // DocFX emits index.html. Redirect explicit index links to the clean directory URL.
            // The canonical URL has the same relative-link base as index.html.
            docs.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value ?? string.Empty;
                if ((HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method)) &&
                    path.EndsWith("/index.html", StringComparison.OrdinalIgnoreCase) &&
                    provider.GetFileInfo(path.TrimStart('/')).Exists)
                {
                    var directory = new PathString(path[..^10]);
                    var location = context.Request.PathBase.Add(directory).ToUriComponent()
                        + context.Request.QueryString.ToUriComponent();
                    context.Response.Redirect(location, permanent: true);
                    return;
                }

                await next();
            });

            var defaultFiles = new DefaultFilesOptions { FileProvider = provider };
            defaultFiles.DefaultFileNames.Clear();
            defaultFiles.DefaultFileNames.Add("index.html");
            docs.UseDefaultFiles(defaultFiles);
            docs.UseStaticFiles(new StaticFileOptions { FileProvider = provider });
            docs.Run(context =>
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return context.Response.WriteAsync("Documentation page not found.");
            });
        });
    }
}
