using System.Collections.Generic;

namespace WebSpaVue.Models
{
  public class VueRoute
  {
    public string Path { get; set; }
    public List<VueRoute> Children { get; set; }
    public string Component { get; set; }
  }
}
