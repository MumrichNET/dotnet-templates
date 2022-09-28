using System.Collections.Generic;

using Mumrich.SpaDevMiddleware;
using Mumrich.SpaDevMiddleware.Contracts;

namespace Mumrich.SpaDevHost
{
  public class HostSettings : ISpaDevServerSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
    public string SpaRootPath { get; set; }
  }
}