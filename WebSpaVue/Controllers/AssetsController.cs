using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace WebSpaVue.Controllers
{
  [ApiController]
  [Route("assets")]
  public class AssetsController : ControllerBase
  {
    private const string AssetsSubPath = "wwwroot/assets";
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AssetsController(IWebHostEnvironment webHostEnvironment)
    {
      _webHostEnvironment = webHostEnvironment;
    }

    private static string AssetsPath { get; } = Path.Combine(Environment.CurrentDirectory, AssetsSubPath);

    [HttpGet]
    [Route("get")]
    public IEnumerable<PhysicalFileResult> Get()
    {
      var foo = new FileExtensionContentTypeProvider();

      return _webHostEnvironment.WebRootFileProvider
        .GetDirectoryContents("assets")
        .Select(fileInfo => PhysicalFile(
          $"/assets/{fileInfo.Name}",
          GetMimeTypeForFileExtension(fileInfo.PhysicalPath)));
    }

    [HttpPost]
    [Route("upload")]
    public async Task UploadAsync()
    {
      // TODO: pass by param
      foreach (var file in Request.Form.Files)
      {
        if (file == null || file.Length == 0)
        {
          continue;
        }

        await using var fileStream = new FileStream(
          Path.Combine(AssetsPath, file.FileName),
          FileMode.Create);

        await file.CopyToAsync(fileStream);
      }
    }

    private static string GetMimeTypeForFileExtension(string filePath)
    {
      const string defaultContentType = "application/octet-stream";

      var provider = new FileExtensionContentTypeProvider();

      if (!provider.TryGetContentType(filePath, out string contentType))
      {
        contentType = defaultContentType;
      }

      return contentType;
    }
  }
}