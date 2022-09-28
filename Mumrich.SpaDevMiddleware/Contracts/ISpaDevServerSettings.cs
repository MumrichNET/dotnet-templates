using System.Collections.Generic;

using Mumrich.SpaDevMiddleware.Models;

namespace Mumrich.SpaDevMiddleware.Contracts
{
  public interface ISpaDevServerSettings
  {
    Dictionary<string, SpaSettings> SinglePageApps { get; set; }

    string SpaRootPath { get; set; }
  }
}
