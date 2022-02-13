using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;

using SpaDevServer;

namespace WebSpaVue.Extensions
{
  public static class WebApplicationExtensions
  {
    public static void MapSinglePageApps(this WebApplication webApplication, Dictionary<string, SpaSettings> singlePageApps)
    {
#if RELEASE
      foreach ((string appPath, SpaSettings spaSettings) in singlePageApps)
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
#else
      webApplication.MapReverseProxy();
#endif
    }
  }
}
