using System.Collections.Generic;

namespace web_spa_vue
{
  public class AppSettings
  {
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
  }
}
