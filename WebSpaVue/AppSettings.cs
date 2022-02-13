using System.Collections.Generic;

using SpaDevServer;
using SpaDevServer.Contracts;

namespace WebSpaVue
{
  public class AppSettings : ISpaDevServerSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
  }
}
