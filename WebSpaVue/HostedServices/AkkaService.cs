using System;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;

using AkkaExt;

using Microsoft.Extensions.Hosting;

using WebSpaVue.Aggregates.FormExample;

namespace WebSpaVue.HostedServices;

public class AkkaService : AkkaServiceBase, IHostedService
{
  private readonly IHostApplicationLifetime _appLifetime;
  private readonly IServiceProvider _serviceProvider;

  public AkkaService(IServiceProvider serviceProvider, IHostApplicationLifetime appLifetime) : base("akka-service", serviceProvider)
  {
    _serviceProvider = serviceProvider;
    _appLifetime = appLifetime;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    var formExampleManager = AkkaSystem.ActorOf(DependencyInjectionResolver.Props<FormExampleManager>());
    var aggregateId = Guid.NewGuid();
    // TODO instanciate aggregates...

    // add a continuation task that will guarantee shutdown of application if ActorSystem terminates
    AkkaSystem.WhenTerminated.ContinueWith(_ => _appLifetime.StopApplication(), cancellationToken);

    return Task.CompletedTask;
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    // strictly speaking this may not be necessary - terminating the ActorSystem would also work
    // but this call guarantees that the shutdown of the cluster is graceful regardless
    await CoordinatedShutdown.Get(AkkaSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
  }
}
