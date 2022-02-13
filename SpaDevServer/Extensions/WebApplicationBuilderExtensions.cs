using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Linq;

using SpaDevServer.Helpers;
using SpaDevServer.HostedServices;

namespace SpaDevServer.Extensions
{
  public static class WebApplicationBuilderExtensions
  {
    private static string GetViteJsYarpConfig(string appPath, string address, Guid? guid = null)
    {
      guid ??= Guid.NewGuid();

      appPath = AppPathHelper.GetValidIntermediateAppPath(appPath);

      string clusterId = $"spa-cluster-{guid}";
      //language=regexp
      const string spaRootExpression = @"^.+\\..+$";
      //language=regexp
      const string spaAssetsExpression = "^(src|node_modules|@[a-zA-Z]+)$";

      return $@"{{
          ""ReverseProxy"": {{
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
                  ""Path"": ""{appPath}{{filename:regex({spaRootExpression})?}}""
                }}
              }},
              ""SpaAssets-{guid}"": {{
                ""ClusterId"": ""{clusterId}"",
                ""Match"": {{
                  ""Path"": ""{appPath}{{name:regex({spaAssetsExpression})}}/{{**any}}""
                }}
              }}
            }}
          }}
        }}";
    }

    public static void RegisterSinglePageAppMiddleware(this WebApplicationBuilder builder, Dictionary<string, SpaSettings> singlePageApps)
    {
#if DEBUG
      builder.Host.ConfigureHostConfiguration(configurationBuilder =>
      {
        var origin = new JObject();

        foreach ((string appPath, SpaSettings spaSettings) in singlePageApps)
        {
          var current = JObject.Parse(GetViteJsYarpConfig(appPath, spaSettings.DevServerAddress));

          origin.Merge(current);
        }

        configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(origin.ToString())));
      });

      var reverseProxyConfig = builder.Configuration.GetSection("ReverseProxy");

      builder.Services.AddHostedService<SpaDevelopmentService>();
      builder.Services.AddReverseProxy().LoadFromConfig(reverseProxyConfig);
#endif
    }
  }
}
