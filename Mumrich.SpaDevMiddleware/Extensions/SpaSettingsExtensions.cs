using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Mumrich.SpaDevMiddleware.Extensions
{
  public static class SpaSettingsExtensions
  {
    private static readonly Regex EnvVarRegex = new("^%.+%$");

    public static ProcessStartInfo GetProcessStartInfo(this SpaSettings spaSettings)
    {
      var arguments = $"/c {spaSettings.StartCommand}";
      var processStartInfo = new ProcessStartInfo("cmd")
      {
        Arguments = arguments,
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), spaSettings.SpaRootPath)
      };

      foreach ((string key, string value) in spaSettings.Environment)
      {
        processStartInfo.Environment[key] = EnvVarRegex.IsMatch(value)
          ? Environment.GetEnvironmentVariable(value.Replace("%", string.Empty))
          : value;
      }

      return processStartInfo;
    }
  }
}
