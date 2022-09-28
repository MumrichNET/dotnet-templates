using System.Collections.Generic;

using Mumrich.SpaDevMiddleware.Contracts;
using Mumrich.SpaDevMiddleware.Models;

namespace Mumrich.SpaDevHost
{
  public class HostSettings : ISpaDevServerSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
    public string SpaRootPath { get; set; }
  }
}