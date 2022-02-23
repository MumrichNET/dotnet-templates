using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;

using AkkaExt;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SpaDevServer.Actors;
using SpaDevServer.Contracts;

namespace SpaDevServer.HostedServices
{
  public class SpaDevelopmentService : AkkaServiceBase, IHostedService
  {
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly Dictionary<string, IActorRef> _processRunners = new();
    private readonly ISpaDevServerSettings _spaDevServerSettings;

    public SpaDevelopmentService(IServiceProvider serviceProvider) : base("spa-development-system", serviceProvider)
    {
      _appLifetime = serviceProvider.GetService<IHostApplicationLifetime>();
      _spaDevServerSettings = serviceProvider.GetService<ISpaDevServerSettings>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      foreach ((string spaPath, SpaSettings spaSettings) in _spaDevServerSettings.SinglePageApps)
      {
        _processRunners.Add(spaPath, AkkaSystem.ActorOf(DependencyInjectionResolver.Props<ProcessRunnerActor>(spaSettings)));
      }

      RegisterApplicationShutdownIfAkkaSystemTerminates(_appLifetime, cancellationToken);

      return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      await GracefullyShutdownAkkaSystemAsync();
    }
  }
}
