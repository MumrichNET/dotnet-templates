using System.Collections.Generic;

namespace Mumrich.SpaDevMiddleware.Domain.Models
{
  public class SpaProxyConfig
  {
    public Dictionary<string, Route> Routes { get; set; }

    public Dictionary<string, Cluster> Clusters { get; set; }
  }
}
