using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

using Helpers;

namespace Mumrich.SpaDevMiddleware.Extensions
{
  public static class SpaSettingsExtensions
  {
    private static readonly Regex EnvVarRegex = new("^%.+%$");

    private static string GetExeName(this SpaSettings spaSettings)
    {
      return spaSettings.NodePackageManager switch
      {
        NodePackageManager.Npm => "npm",
        NodePackageManager.Yarn => "yarn",
        NodePackageManager.Npx => "npx",
        NodePackageManager.Pnpm => "pnpm",
        _ => "npm",
      };
    }

    private static string BuildCommand(this SpaSettings spaSettings, string arguments = null)
    {
      var command = new StringBuilder();
      var isNpm = spaSettings.NodePackageManager == NodePackageManager.Npm;

      if (isNpm)
      {
        command.Append("run ");
      }

      command.Append(spaSettings.NodeStartScript);
      command.Append(' ');

      if (isNpm)
      {
        command.Append("-- ");
      }

      if (!string.IsNullOrWhiteSpace(arguments))
      {
        command.Append(arguments);
      }

      return command.ToString();
    }

    private static (string, string) GetCompleteCommand(this SpaSettings spaSettings)
    {
      string completeArguments = spaSettings.BuildCommand();
      string exeName = spaSettings.GetExeName();

      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        return (exeName, completeArguments);
      }

      // On Windows, the NPM executable is a .cmd file, so it can't be executed
      // directly (except with UseShellExecute=true, but that's no good, because
      // it prevents capturing stdio). So we need to invoke it via "cmd /c".
      completeArguments = $"/c {exeName} {completeArguments}";
      exeName = "cmd";

      return (exeName, completeArguments);
    }

    public static ProcessStartInfo GetProcessStartInfo(this SpaSettings spaSettings)
    {
      (string exeName, string completeArguments) = spaSettings.GetCompleteCommand();
      var processStartInfo = new ProcessStartInfo(exeName)
      {
        Arguments = completeArguments,
        UseShellExecute = false,
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        WorkingDirectory = DirPathHelper.CombineToFullPath(Directory.GetCurrentDirectory(), spaSettings.SpaRootPath)
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
