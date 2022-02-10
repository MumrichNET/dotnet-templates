using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Linq;

using SpaDevServer.HostedServices;

namespace web_spa_vue.Extensions
{
  public static class WebApplicationBuilderExtensions
  {
    private static string GetViteJsYarpConfig(string appPath, string address, Guid? guid = null)
    {
      guid ??= Guid.NewGuid();

      if (string.IsNullOrWhiteSpace(appPath))
      {
        appPath = "/";
      }
      else
      {
        if (!appPath.StartsWith("/"))
        {
          appPath = $"/{appPath}";
        }

        if (!appPath.EndsWith("/"))
        {
          appPath = $"{appPath}/";
        }
      }

      string clusterId = $"spa-cluster-{guid}";

      return $@"{{
          ""Clusters"": {{
            ""{clusterId}"": {{
              ""Destinations"": {{
                ""spa-cluster-destination-{guid}"": {{
                  ""Address"": ""{address}""
                }}
              }}
            }}
          }},
          ""Routes"": {{
            ""SpaRoot-{guid}"": {{
              ""ClusterId"": ""{clusterId}"",
              ""Match"": {{
                ""Path"": ""{appPath}{{filename:regex(^.+\\..+$)?}}""
              }}
            }},
            ""SpaAssets-{guid}"": {{
              ""ClusterId"": ""{clusterId}"",
              ""Match"": {{
                ""Path"": ""{appPath}{{name:regex(^(src|node_modules|@vite|@id)$)}}/{{**any}}""
              }}
            }}
          }}
        }}";
    }

    public static WebApplicationBuilder RegisterSinglePageAppMiddleware(this WebApplicationBuilder builder, Dictionary<string, SpaSettings> singlePageApps)
    {
#if DEBUG
      var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy");

      // TODO: write own configuration-provider...
      //var original = reverseProxyConfig.Value != null ? JObject.Parse(reverseProxyConfig.Value) : new JObject();

      //foreach ((string appPath, SpaSettings spaSettings) in singlePageApps)
      //{
      //  var spaConfig = JObject.Parse(GetViteJsYarpConfig(appPath, spaSettings.DevServerAddress));

      //  original.Merge(spaConfig);
      //}

      //reverseProxyConfig.Value = original.ToString();

      builder.Services.AddHostedService<ViteJsDevelopmentService>();
      builder.Services.AddReverseProxy().LoadFromConfig(reverseProxyConfig);
#endif

      return builder;
    }
  }
}
