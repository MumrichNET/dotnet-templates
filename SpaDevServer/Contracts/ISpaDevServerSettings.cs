using System.Collections.Generic;

namespace SpaDevServer.Contracts
{
  public interface ISpaDevServerSettings
  {
    Dictionary<string, SpaSettings> SinglePageApps { get; set; }
  }
}
