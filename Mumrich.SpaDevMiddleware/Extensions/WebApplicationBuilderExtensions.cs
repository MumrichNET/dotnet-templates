using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Mumrich.SpaDevMiddleware.Contracts;
using Mumrich.SpaDevMiddleware.Helpers;
using Mumrich.SpaDevMiddleware.HostedServices;

using Newtonsoft.Json.Linq;

namespace Mumrich.SpaDevMiddleware.Extensions
{
  public static class WebApplicationBuilderExtensions
  {
    public static void RegisterSinglePageAppDevMiddleware(this WebApplicationBuilder builder, ISpaDevServerSettings spaDevServerSettings)
    {
      if (!builder.Environment.IsDevelopment())
      {
        return;
      }

      builder.Host.ConfigureHostConfiguration(configurationBuilder =>
      {
        var origin = new JObject();

        foreach ((string appPath, SpaSettings spaSettings) in spaDevServerSettings.SinglePageApps)
        {
          var guid = Guid.NewGuid();
          var current = JObject.Parse(spaSettings.Bundler switch
          {
            BundlerType.ViteJs => GetViteJsYarpConfig(appPath, guid, spaSettings),
            BundlerType.QuasarCli => GetQuasarYarpConfig(appPath, guid, spaSettings),
            _ => throw new NotImplementedException()
          });

          origin.Merge(current);
        }

        var newConfig = origin.ToString();

        configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(newConfig)));
      });

      var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy");

      builder.Services.AddSingleton(spaDevServerSettings);
      builder.Services.AddHostedService<SpaDevelopmentService>();
      builder.Services.AddReverseProxy().LoadFromConfig(reverseProxyConfig);
    }

    private static string GetQuasarYarpConfig(string appPath, Guid guid, SpaSettings spaSettings)
    {
      return GetYarpConfig(
        appPath,
        spaSettings,
        new Dictionary<string, string>
        {
          {
            $"SpaRoot-{guid}", "{**any}"
          }
        },
        guid);
    }

    private static string GetViteJsYarpConfig(string appPath, Guid guid, SpaSettings spaSettings)
    {
      return GetYarpConfig(
        appPath,
        spaSettings,
        new Dictionary<string, string>
        {
          {
            $"SpaRoot-{guid}", $"{{filename:regex({spaSettings.SpaRootExpression})?}}"
          },
          {
            $"SpaAssets-{guid}", $"{{name:regex({spaSettings.SpaAssetsExpression})}}/{{**any}}"
          }
        },
        guid);
    }

    private static string GetYarpConfig(string appPath, SpaSettings spaSettings, Dictionary<string, string> routeMatches, Guid guid)
    {
      appPath = AppPathHelper.GetValidIntermediateAppPath(appPath);
      string clusterId = $"spa-cluster-{guid}";

      var rootConfig = JObject.Parse($@"{{
          ""ReverseProxy"": {{
            ""Clusters"": {{
              ""{clusterId}"": {{
                ""Destinations"": {{
                  ""spa-cluster-destination-{guid}"": {{
                    ""Address"": ""{spaSettings.DevServerAddress}""
                  }}
                }}
              }}
            }}
          }}
        }}");

      foreach ((string route, string path) in routeMatches)
      {
        rootConfig.Merge(JObject.Parse(GetYarpRoute(route, clusterId, appPath + path, spaSettings)));
      }

      return rootConfig.ToString();
    }

    private static string GetYarpRoute(string route, string clusterId, string path, SpaSettings spaSettings)
    {
      return $@"{{
        ""ReverseProxy"": {{
          ""Routes"": {{
            ""{route}"": {{
              ""ClusterId"": ""{clusterId}"",
              ""AuthorizationPolicy"": ""{spaSettings.AuthorizationPolicy}"",
              ""CorsPolicy"": ""{spaSettings.CorsPolicy}"",
              ""Match"": {{
                ""Path"": ""{path}""
              }}
            }}
          }}
        }}
      }}";
    }
  }
}
