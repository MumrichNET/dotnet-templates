using System.Collections.Generic;

namespace Mumrich.SpaDevMiddleware;

public enum BundlerType
{
  ViteJs,
  QuasarCli
}

public class SpaSettings
{
  /// <summary>
  /// The env-vars to pass to the dev-server-process.
  /// Values can be env-vars themselves and must be surrounded with '%'
  /// E. g.: %ASPNETCORE_HTTPS_PORT%, etc.
  /// </summary>
  public Dictionary<string, string> Environment = new();

  /// <summary>
  /// The Bundler used by the SPA.
  /// E. g.: Vue.js, Quasar-CLI, etc.
  /// </summary>
  public BundlerType Bundler { get; set; } = BundlerType.ViteJs;

  /// <summary>
  /// The full Url to the dev-server.
  /// E. g.: "http://localhost:8080/" or "https://app:3000/"
  /// </summary>
  public string DevServerAddress { get; set; }

  /// <summary>
  /// A regular-expression that matches when the dev-server has successfully started.
  /// </summary>
  public string Regex { get; set; }

  /// <summary>
  /// The path to the app root-dir, relative to the current directory.
  /// E. g.: "client-app" assuming the SPA resides in "%ProjectDir%/client-app"
  /// </summary>
  public string SpaRootPath { get; set; }

  /// <summary>
  /// The star-command for the dev-server to launch
  /// E. g.: 'npm run dev' or 'yarn run', etc.
  /// </summary>
  public string StartCommand { get; set; }

  /// <summary>
  /// Name of the policy or "Default", "Anonymous"
  /// </summary>
  public string AuthorizationPolicy { get; set; } = "Anonymous";

  /// <summary>
  /// Name of the CorsPolicy to apply to this route or "Default", "Disable"
  /// </summary>
  public string CorsPolicy { get; set; } = "Default";
}
