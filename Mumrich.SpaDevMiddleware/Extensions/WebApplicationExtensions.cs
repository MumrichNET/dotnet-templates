using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

using Mumrich.SpaDevMiddleware.Contracts;
using Mumrich.SpaDevMiddleware.Helpers;

namespace Mumrich.SpaDevMiddleware.Extensions
{
  public static class WebApplicationExtensions
  {
    public static void MapSinglePageApps(this WebApplication webApplication, ISpaDevServerSettings spaDevServerSettings)
    {
      if (!webApplication.Environment.IsDevelopment())
      {
        foreach ((string appPath, SpaSettings spaSettings) in spaDevServerSettings.SinglePageApps)
        {
          webApplication.MapSinglePageApp(appPath, spaSettings);
        }
      }
      else
      {
        webApplication.MapReverseProxy();
      }
    }

    public static void MapSinglePageApp(this WebApplication webApplication, string appPath, SpaSettings spaSettings)
    {
      var clientAppRoot = Path.Combine(webApplication.Environment.ContentRootPath, $"{spaSettings.SpaRootPath}/dist");
      webApplication.UseStaticFiles(new StaticFileOptions
      {
        FileProvider = new PhysicalFileProvider(clientAppRoot),
        RequestPath = ""
      });

      var clientAppIndex = Path.Combine(clientAppRoot, "index.html");

      webApplication.MapGet(
        AppPathHelper.GetValidIntermediateAppPath(appPath),
        async context => await context.Response.SendFileAsync(clientAppIndex));
    }
  }
}
