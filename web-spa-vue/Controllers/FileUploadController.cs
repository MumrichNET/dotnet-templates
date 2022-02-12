using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace web_spa_vue.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class FileUploadController : ControllerBase
  {
    [HttpPost]
    public async Task Upload()
    {
      foreach (var file in Request.Form.Files)
      {
        if (file == null || file.Length == 0)
        {
          continue;
        }

        var fileName = file.FileName;
        var fileSize = file.Length;
      }
    }
  }
}
