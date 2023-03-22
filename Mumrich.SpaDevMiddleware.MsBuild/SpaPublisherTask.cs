using System.Text.Json;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using MsBuildTask = Microsoft.Build.Utilities.Task;

namespace Mumrich.SpaDevMiddleware.MSBuild
{
  public class MiniSettings
  {
    // ReSharper disable once CollectionNeverUpdated.Global
    public Dictionary<string, MiniSpaSettings> SinglePageApps { get; set; } = new();
  }

  // ReSharper disable once ClassNeverInstantiated.Global
  public class MiniSpaSettings
  {
    public string NodeBuildScript { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string SpaRootPath { get; set; }
  }

  // ReSharper disable once UnusedType.Global
  public class SpaPublisherTask : MsBuildTask
  {
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string AspNetCoreEnvironment { get; set; }

    [Output]
    public TaskItem[] SpaPaths { get; set; }

    public override bool Execute()
    {
      var currentDir = Directory.GetCurrentDirectory();
      var defaultAppSettingsFile = Directory
        .GetFiles(currentDir, "appsettings.json")
        .FirstOrDefault();
      var envAppSettingsFile = Directory
        .GetFiles(currentDir, $"appsettings.{AspNetCoreEnvironment}.json")
        .FirstOrDefault();

      var defaultAppSettings = TryReadMiniSettings(defaultAppSettingsFile);
      var envAppSettings = TryReadMiniSettings(envAppSettingsFile);
      var spaSettings = new Dictionary<string, MiniSpaSettings>();

      CollectSpaPaths(nameof(envAppSettings), spaSettings, envAppSettings);
      CollectSpaPaths(nameof(defaultAppSettings), spaSettings, defaultAppSettings);

      SpaPaths = spaSettings.Values.Select(v => new TaskItem(
        ConvertToMsBuildCompatiblePath(v.SpaRootPath),
        new Dictionary<string, string>()
        {
          { "NodeBuildScript", v.NodeBuildScript }
        })).ToArray();

      return true;
    }

    private static string ConvertToMsBuildCompatiblePath(string spaRootPath)
    {
      var spaRootPathWithBackslashed = spaRootPath.Replace("/", "\\");
      return spaRootPathWithBackslashed.EndsWith("\\") ? spaRootPathWithBackslashed : $"{spaRootPathWithBackslashed}\\";
    }

    private static MiniSettings TryReadMiniSettings(string filePath)
    {
      if (filePath is null)
      {
        return new();
      }

      var fileText = File.ReadAllText(filePath);
      return JsonSerializer.Deserialize<MiniSettings>(fileText);
    }

    private void CollectSpaPaths(string prefix, Dictionary<string, MiniSpaSettings> spaSettings, MiniSettings miniSettings)
    {
      foreach ((string route, MiniSpaSettings value) in miniSettings.SinglePageApps)
      {
        var spaDirectory = value.SpaRootPath;

        if (spaSettings.TryAdd(route, value))
        {
          LogImportantMessage($"{prefix} => '{route}': '{spaDirectory}'");
        }
      }
    }

    private void LogImportantMessage(string message)
    {
      Log.LogMessage(MessageImportance.High, message);
    }
  }
}