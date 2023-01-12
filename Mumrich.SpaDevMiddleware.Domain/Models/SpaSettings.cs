using System.Collections.Generic;

using Mumrich.SpaDevMiddleware.Domain.Types;

namespace Mumrich.SpaDevMiddleware.Domain.Models
{
  public class SpaSettings
  {
    /// <summary>
    /// The env-vars to pass to the dev-server-process.
    /// Values can be env-vars themselves and must be surrounded with '%'
    /// E. g.: %ASPNETCORE_HTTPS_PORT%, etc.
    /// </summary>
    public Dictionary<string, string> Environment = new Dictionary<string, string>();

    /// <summary>
    /// Name of the policy or "Default", "Anonymous"
    /// </summary>
    public string AuthorizationPolicy { get; set; } = "Anonymous";

    /// <summary>
    /// The Bundler used by the SPA.
    /// E. g.: Vue.js, Quasar-CLI, etc.
    /// </summary>
    public BundlerType Bundler { get; set; } = BundlerType.ViteJs;

    /// <summary>
    /// Name of the CorsPolicy to apply to this route or "Default", "Disable"
    /// </summary>
    public string CorsPolicy { get; set; } = "Default";

    public SpaProxyConfig CustomYarpConfiguration { get; set; }

    /// <summary>
    /// The full Url to the dev-server.
    /// E. g.: "http://localhost:8080/" or "https://app:3000/"
    /// </summary>
    public string DevServerAddress { get; set; }

    /// <summary>
    /// The npm-compatible package-manager to use.
    /// </summary>
    public NodePackageManager NodePackageManager { get; set; } = NodePackageManager.Yarn;

    /// <summary>
    /// The star-command for the dev-server to launch
    /// E. g.: 'npm run dev' or 'yarn run', etc.
    /// </summary>
    public string NodeStartScript { get; set; } = "dev";

    /// <summary>
    /// The build-commmand for the spa to build/publish
    /// E. g.: 'npm run build' or 'yarn build', etc.
    /// </summary>
    public string NodeBuildScript { get; set; } = "build";

    /// <summary>
    /// A regular-expression that matches when the dev-server has successfully started.
    /// </summary>
    public string Regex { get; set; }

    /// <summary>
    /// The RegExp for detecting SPA-Assets requests.
    /// </summary>
    //language=regexp
    public string SpaAssetsExpression { get; set; } = "^(src|node_modules|@[a-zA-Z]+|.*vite.*)$";

    /// <summary>
    /// The RegExp for detecting SPA-Root requests.
    /// </summary>
    //language=regexp
    public string SpaRootExpression { get; set; } = @"^.+\\..+$";

    /// <summary>
    /// The path to the app root-dir, relative to the current directory.
    /// E. g.: "client-app" assuming the SPA resides in "%ProjectDir%/client-app"
    /// </summary>
    public string SpaRootPath { get; set; }
  }
}