using System.Collections.Generic;

using SpaDevServer;
using SpaDevServer.Contracts;

namespace web_spa_vue
{
  public class AppSettings : ISpaDevServerSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
  }
}
