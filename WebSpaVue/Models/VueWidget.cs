using System.Collections.Generic;

namespace WebSpaVue.Models
{
  public class VueWidget
  {
    public string Component { get; set; }
    public Dictionary<string, object> Props { get; set; }
  }
}
