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
}