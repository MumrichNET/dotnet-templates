using System.Collections.Generic;

using Mumrich.SpaDevMiddleware;
using Mumrich.SpaDevMiddleware.Contracts;

namespace Template.WebSpaVue3Empty;

public class AppSettings : ISpaDevServerSettings
{
  public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
}