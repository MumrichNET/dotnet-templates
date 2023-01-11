using System.Collections.Generic;

using Mumrich.SpaDevMiddleware.Models;

namespace Mumrich.SpaDevMiddleware.Contracts
{
  public interface ISpaDevServerSettings
  {
    Dictionary<string, SpaSettings> SinglePageApps { get; set; }

    string SpaRootPath { get; set; }

    /// <summary>
    /// [ONLY ON WINDOWS]: Currently, Visual Studio shows the unfortunate behavior that it kills the child-process when the debugging is stopped. This prevents a proper shutdown of the child (node.js) processes that are run by this middleware.
    ///
    /// To the knowledge of the author, there currently exists no simple way to detect when a debugging-session is killed by Visual Studio. Therefore, the SpaDevMiddleware is run within it's own dotnet-process, thus preventing to be killed by Visual Studio. This allows the middleware to observe the state of it's parent process and to shut down gracefully as soon as the parent process is not alive anymore.
    ///
    /// !! Currently, this mechanism makes use of the Windows Management Instrumentation (WMI) and therefore only runs on Windows !!
    /// </summary>
    bool UseParentObserverServiceOnWindows { get; set; }
  }
}