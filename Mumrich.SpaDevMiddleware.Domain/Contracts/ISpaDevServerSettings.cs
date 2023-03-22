using System.Collections.Generic;

using Mumrich.SpaDevMiddleware.Domain.Models;

namespace Mumrich.SpaDevMiddleware.Domain.Contracts
{
  public interface ISpaDevServerSettings
  {
    /// <summary>
    /// All configurations of all single-page-apps, identified by their url-path.
    /// </summary>
    Dictionary<string, SpaSettings> SinglePageApps { get; set; }

    /// <summary>
    /// The root-path where the single-page-apps are contained.
    /// </summary>
    string SpaRootPath { get; set; }

    /// <summary>
    /// <para>[ONLY ON WINDOWS]: Currently, some IDEs (e. g. Visual Studio 2022, VS Code) show the unfortunate behavior that they forcefully shutdown (kill) the child-process when the debugging is stopped. This prevents a proper/gracefull shutdown of the child processes (e. g. node.js) that are invoked and run by this middleware.</para>
    /// <para>To the best knowledge of the author of this libraray, there currently exists no simple way to detect when a debugging-session is killed by Visual Studio (like event-hook). Therefore, the SpaDevMiddleware is run within it's own dotnet-process, thus preventing to be killed by Visual Studio. This allows the middleware to observe the state of it's parent process and to shut down gracefully as soon as the parent process is not alive anymore.</para>
    /// <para>!! Currently, this mechanism makes use of the Windows Management Instrumentation (WMI) and therefore only runs on Windows !!</para>
    /// </summary>
    bool UseParentObserverServiceOnWindows { get; set; }
  }
}