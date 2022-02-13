using System.Collections.Generic;

namespace SpaDevServer;

public class SpaSettings
{
  /// <summary>
  /// The path to the app root-dir, relative to the current directory.
  /// E. g.: "client-app" assuming the SPA resides in "%ProjectDir%/client-app"
  /// </summary>
  public string SpaRootPath { get; set; }

  /// <summary>
  /// The full Url to the dev-server.
  /// E. g.: "http://localhost:8080/" or "https://app:3000/"
  /// </summary>
  public string DevServerAddress { get; set; }

  /// <summary>
  /// The star-command for the dev-server to launch
  /// E. g.: 'npm run dev' or 'yarn run', etc.
  /// </summary>
  public string StartCommand { get; set; }

  /// <summary>
  /// The env-vars to pass to the dev-server-process.
  /// Values can be env-vars themselves and must be surrounded with '%'
  /// E. g.: %ASPNETCORE_HTTPS_PORT%, etc.
  /// </summary>
  public Dictionary<string, string> Environment = new();

  /// <summary>
  /// A regular-expression that matches when the dev-server has successfully started.
  /// </summary>
  public string Regex { get; set; }
}