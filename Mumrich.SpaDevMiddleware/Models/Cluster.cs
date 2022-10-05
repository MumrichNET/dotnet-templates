using System.Collections.Generic;

namespace Mumrich.SpaDevMiddleware.Models;

public class Cluster
{
  public Dictionary<string, Destination> Destinations { get; set; }
}