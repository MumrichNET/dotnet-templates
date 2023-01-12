using System.Collections.Generic;
using System.IO;

using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.Domain.Models;

namespace WebSpaVue
{
  public class AppSettings : ISpaDevServerSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
    public string SpaRootPath { get; set; } = Directory.GetCurrentDirectory();
    public bool UseParentObserverServiceOnWindows { get; set; } = true;
  }
}