using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Mumrich.HelpersAndExtensions;

namespace Mumrich.SpaDevMiddleware.HostedServices
{
  public sealed class WindowsParentProcessObserverService : IHostedService
  {
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger _logger;

    public WindowsParentProcessObserverService(ILogger<WindowsParentProcessObserverService> logger, IHostApplicationLifetime appLifetime)
    {
      _logger = logger;
      _appLifetime = appLifetime;
      appLifetime.ApplicationStarted.Register(OnStarted);
      appLifetime.ApplicationStopping.Register(OnStopping);
      appLifetime.ApplicationStopped.Register(OnStopped);

      Process self = Process.GetCurrentProcess();
      Process parent = self.GetParentProcess();

      _logger.LogInformation("parent: {}", parent.ProcessName);

      parent.EnableRaisingEvents = true;
      parent.ErrorDataReceived += Parent_Exited;
      parent.Disposed += Parent_Exited;
      parent.Exited += Parent_Exited;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("1. StartAsync has been called.");

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("4. StopAsync has been called.");

      return Task.CompletedTask;
    }

    private void OnStarted()
    {
      _logger.LogInformation("2. OnStarted has been called.");
    }

    private void OnStopped()
    {
      _logger.LogInformation("5. OnStopped has been called.");
    }

    private void OnStopping()
    {
      _logger.LogInformation("3. OnStopping has been called.");
    }

    private void Parent_Exited(object sender, System.EventArgs e)
    {
      _logger.LogCritical("Parent is dead!");
      _appLifetime.StopApplication();
    }
  }
}