using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json.Linq;

using SpaDevServer.Helpers;
using SpaDevServer.HostedServices;

namespace SpaDevServer.Extensions
{
  public static class WebApplicationBuilderExtensions
  {
    public static void RegisterSinglePageAppDevMiddleware(this WebApplicationBuilder builder, Dictionary<string, SpaSettings> singlePageApps)
    {
      if (builder.Environment.IsDevelopment())
      {
        builder.Host.ConfigureHostConfiguration(configurationBuilder =>
      {
        var origin = new JObject();

        foreach ((string appPath, SpaSettings spaSettings) in singlePageApps)
        {
          var guid = Guid.NewGuid();
          var current = JObject.Parse(spaSettings.Bundler switch
          {
            BundlerType.ViteJs => GetViteJsYarpConfig(appPath, spaSettings.DevServerAddress, guid),
            BundlerType.QuasarCli => GetQuasarYarpConfig(appPath, spaSettings.DevServerAddress, guid),
            _ => throw new NotImplementedException()
          });

          origin.Merge(current);
        }

        var newConfig = origin.ToString();

        configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(newConfig)));
      });

        var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy");

        builder.Services.AddHostedService<SpaDevelopmentService>();
        builder.Services.AddReverseProxy().LoadFromConfig(reverseProxyConfig);
      }
    }

    private static string GetQuasarYarpConfig(string appPath, string address, Guid guid)
    {
      return GetYarpConfig(appPath, address, new Dictionary<string, string>
      {
        {
          $"SpaRoot-{guid}", "{**any}"
        }
      }, guid);
    }

    private static string GetViteJsYarpConfig(string appPath, string address, Guid guid)
    {
      //language=regexp
      const string spaRootExpression = @"^.+\\..+$";
      //language=regexp
      const string spaAssetsExpression = "^(src|node_modules|@[a-zA-Z]+)$";

      return GetYarpConfig(appPath, address, new Dictionary<string, string>
      {
        {
          $"SpaRoot-{guid}", $"{{filename:regex({spaRootExpression})?}}"
        },
        {
          $"SpaAssets-{guid}", $"{{name:regex({spaAssetsExpression})}}/{{**any}}"
        }
      }, guid);
    }

    private static string GetYarpConfig(string appPath, string address, Dictionary<string, string> routeMatches, Guid guid)
    {
      appPath = AppPathHelper.GetValidIntermediateAppPath(appPath);
      string clusterId = $"spa-cluster-{guid}";

      var rootConfig = JObject.Parse($@"{{
          ""ReverseProxy"": {{
            ""Clusters"": {{
              ""{clusterId}"": {{
                ""Destinations"": {{
                  ""spa-cluster-destination-{guid}"": {{
                    ""Address"": ""{address}""
                  }}
                }}
              }}
            }}
          }}
        }}");

      foreach ((string route, string path) in routeMatches)
      {
        rootConfig.Merge(JObject.Parse(GetYarpRoute(route, clusterId, appPath + path)));
      }

      return rootConfig.ToString();
    }

    private static string GetYarpRoute(string route, string clusterId, string path)
    {
      return $@"{{
        ""ReverseProxy"": {{
          ""Routes"": {{
            ""{route}"": {{
              ""ClusterId"": ""{clusterId}"",
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