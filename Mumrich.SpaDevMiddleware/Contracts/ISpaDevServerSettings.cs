using System.Collections.Generic;

namespace Mumrich.SpaDevMiddleware.Contracts
{
  public interface ISpaDevServerSettings
  {
    Dictionary<string, SpaSettings> SinglePageApps { get; set; }
  }
}
