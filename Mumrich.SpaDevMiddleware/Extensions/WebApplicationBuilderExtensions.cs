using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.Domain.Models;
using Mumrich.SpaDevMiddleware.Domain.Types;
using Mumrich.SpaDevMiddleware.Helpers;
using Mumrich.SpaDevMiddleware.HostedServices;

using Newtonsoft.Json.Linq;

namespace Mumrich.SpaDevMiddleware.Extensions
{
  public static class WebApplicationBuilderExtensions
  {
    [SuppressMessage("Usage", "ASP0013:Suggest switching from using Configure methods to WebApplicationBuilder.Configuration")]
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
          Guid guid = Guid.NewGuid();
          JObject current = spaSettings.Bundler switch
          {
            BundlerType.ViteJs => GetViteJsYarpConfig(appPath, guid, spaSettings),
            BundlerType.QuasarCli => GetQuasarYarpConfig(appPath, guid, spaSettings),
            BundlerType.Custom => JObject.FromObject(new { ReverseProxy = spaSettings.CustomYarpConfiguration }),
            _ => throw new NotImplementedException()
          };

          origin.Merge(current);
        }

        var newConfig = origin.ToString();

        Console.WriteLine(newConfig);

        configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(newConfig)));
      });

      var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy");

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && spaDevServerSettings.UseParentObserverServiceOnWindows)
      {
        builder.Services.AddHostedService<AkkaHostParentService>();
      }
      else
      {
        builder.Services.AddHostedService<SpaDevelopmentService>();
      }

      builder.Services.AddReverseProxy().LoadFromConfig(reverseProxyConfig);
    }

    private static JObject GetQuasarYarpConfig(string appPath, Guid guid, SpaSettings spaSettings)
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

    private static JObject GetViteJsYarpConfig(string appPath, Guid guid, SpaSettings spaSettings)
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

    private static JObject GetYarpConfig(
      string appPath,
      SpaSettings spaSettings,
      Dictionary<string, string> routeMatches,
      Guid guid)
    {
      appPath = AppPathHelper.GetValidIntermediateAppPath(appPath);
      string clusterId = $"spa-cluster-{guid}";
      JObject rootConfig = JObject.FromObject(new
      {
        ReverseProxy = new
        {
          Clusters = new Dictionary<string, object>
            {
              {
                clusterId,
                new
                {
                  Destinations = new Dictionary<string, object>
                  {
                    {
                      $"spa-cluster-destination-{guid}",
                      new
                      {
                        Address = spaSettings.DevServerAddress
                      }
                    }
                  }
                }
              }
            }
        }
      });

      foreach ((string route, string path) in routeMatches)
      {
        rootConfig.Merge(GetYarpRoute(route, clusterId, appPath + path, spaSettings));
      }

      return rootConfig;
    }

    private static JObject GetYarpRoute(string route, string clusterId, string path, SpaSettings spaSettings)
    {
      return JObject.FromObject(new
      {
        ReverseProxy = new
        {
          Routes = new Dictionary<string, object>
          {
            {
              route,
              new
              {
                ClusterId = clusterId,
                spaSettings.AuthorizationPolicy,
                spaSettings.CorsPolicy,
                Match = new
                {
                  Path = path
                }
              }
            }
          }
        }
      });
    }
  }
}