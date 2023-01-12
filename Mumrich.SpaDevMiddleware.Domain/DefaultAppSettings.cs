using System.Collections.Generic;
using System.IO;

using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.Domain.Models;

namespace Mumrich.SpaDevMiddleware.Domain
{
  public class DefaultAppSettings : ISpaDevServerSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
    public string SpaRootPath { get; set; } = Directory.GetCurrentDirectory();
    public bool UseParentObserverServiceOnWindows { get; set; } = false;
  }
}