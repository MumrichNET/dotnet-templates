using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Mumrich.AkkaExt;
using Mumrich.SpaDevMiddleware.Actors;
using Mumrich.SpaDevMiddleware.Contracts;

namespace Mumrich.SpaDevMiddleware.HostedServices
{
  public class SpaDevelopmentService : AkkaServiceBase, IHostedService
  {
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly Dictionary<string, IActorRef> _processRunners = new();
    private readonly ISpaDevServerSettings _spaDevServerSettings;

    public SpaDevelopmentService(IServiceProvider serviceProvider) : base("spa-development-system", serviceProvider)
    {
      _appLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
      _spaDevServerSettings = serviceProvider.GetRequiredService<ISpaDevServerSettings>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _appLifetime.ApplicationStopping.Register(() =>
      {
        Console.WriteLine("ApplicationStopping called");
      });

      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

      foreach ((string spaPath, SpaSettings spaSettings) in _spaDevServerSettings.SinglePageApps)
      {
        _processRunners.Add(spaPath, AkkaSystem.ActorOf(DependencyInjectionResolver.Props<ProcessRunnerActor>(spaSettings)));
      }

      RegisterApplicationShutdownIfAkkaSystemTerminates(_appLifetime, cancellationToken);

      return Task.CompletedTask;
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
      Console.WriteLine("CurrentDomain_ProcessExit");
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      Console.WriteLine("CurrentDomain_UnhandledException");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      await GracefullyShutdownAkkaSystemAsync();

      await CoordinatedShutdown
        .Get(AkkaSystem)
        .Run(CoordinatedShutdown.ClrExitReason.Instance);

      AkkaSystem.Dispose();
    }
  }
}
