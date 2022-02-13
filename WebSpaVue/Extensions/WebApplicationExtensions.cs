using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

using SpaDevServer;

using web_spa_vue.Helpers;

namespace web_spa_vue.Extensions
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
