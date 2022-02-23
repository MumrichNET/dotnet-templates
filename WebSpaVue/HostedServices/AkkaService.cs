using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using Mumrich.AkkaExt;

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

    RegisterApplicationShutdownIfAkkaSystemTerminates(_appLifetime, cancellationToken);

    return Task.CompletedTask;
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    await GracefullyShutdownAkkaSystemAsync();
  }
}
