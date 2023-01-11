using System.Collections.Generic;
using System.IO;

using Mumrich.SpaDevMiddleware.Contracts;
using Mumrich.SpaDevMiddleware.Models;

namespace Mumrich.SpaDevMiddleware
{
  public class AppSettings : ISpaDevServerSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
    public string SpaRootPath { get; set; } = Directory.GetCurrentDirectory();
    public bool UseParentObserverServiceOnWindows { get; set; } = false;
  }
}