using System.Collections.Generic;

namespace Mumrich.SpaDevMiddleware.Domain.Models
{
  public class Cluster
  {
    public Dictionary<string, Destination> Destinations { get; set; }
  }
}
