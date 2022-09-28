using System.Collections.Generic;
using System.IO;

using Mumrich.SpaDevMiddleware;
using Mumrich.SpaDevMiddleware.Contracts;

namespace WebSpaVue
{
  public class AppSettings : ISpaDevServerSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
    public string SpaRootPath { get; set; } = Directory.GetCurrentDirectory();
  }
}
