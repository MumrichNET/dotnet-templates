using System;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mumrich.AkkaExt
{
  public abstract class AkkaServiceBase
  {
    private readonly ILogger<AkkaServiceBase> _logger;

    protected AkkaServiceBase(string actorSystemName, IServiceProvider serviceProvider, Config config = null)
    {
      if (actorSystemName == null)
      {
        throw new ArgumentException("Provide a name for the Actor-System!");
      }

      ServiceProvider = serviceProvider;

      _logger = ServiceProvider.GetService<ILogger<AkkaServiceBase>>();

      config ??= ConfigurationFactory.Default();

      var bootstrap = BootstrapSetup.Create().WithConfig(config);
      var diSetup = DependencyResolverSetup.Create(ServiceProvider);
      var actorSystemSetup = bootstrap.And(diSetup);

      AkkaSystem = ActorSystem.Create(actorSystemName, actorSystemSetup);
      DependencyInjectionResolver = DependencyResolver.For(AkkaSystem);

      AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    private void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
      _logger.LogInformation(nameof(CurrentDomain_ProcessExit));

      // TODO: better/cleaner awaiter...
      GracefullyShutdownAkkaSystemAsync().GetAwaiter().GetResult();
    }

    protected ActorSystem AkkaSystem { get; }
    protected DependencyResolver DependencyInjectionResolver { get; }
    protected IServiceProvider ServiceProvider { get; }

    protected async Task GracefullyShutdownAkkaSystemAsync()
    {
      _logger.LogInformation(nameof(GracefullyShutdownAkkaSystemAsync));

      // strictly speaking this may not be necessary - terminating the ActorSystem would also work
      // but this call guarantees that the shutdown of the cluster is graceful regardless
      await CoordinatedShutdown
        .Get(AkkaSystem)
        .Run(CoordinatedShutdown.ClrExitReason.Instance);
    }

    protected void RegisterApplicationShutdownIfAkkaSystemTerminates(IHostApplicationLifetime appLifetime, CancellationToken cancellationToken)
    {
      // add a continuation task that will guarantee shutdown of application if ActorSystem terminates
      AkkaSystem.WhenTerminated.ContinueWith(_ => appLifetime.StopApplication(), cancellationToken);
    }
  }
}
