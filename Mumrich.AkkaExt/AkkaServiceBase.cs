using System;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;

using Microsoft.Extensions.Hosting;

namespace Mumrich.AkkaExt
{
  public abstract class AkkaServiceBase
  {
    protected AkkaServiceBase(string actorSystemName, IServiceProvider serviceProvider, Config config = null)
    {
      if (actorSystemName == null)
      {
        throw new ArgumentException("Provide a name for the Actor-System!");
      }

      ServiceProvider = serviceProvider;

      config ??= ConfigurationFactory.Default();
      var bootstrap = BootstrapSetup.Create().WithConfig(config);
      var diSetup = DependencyResolverSetup.Create(ServiceProvider);
      var actorSystemSetup = bootstrap.And(diSetup);

      AkkaSystem = ActorSystem.Create(actorSystemName, actorSystemSetup);
      DependencyInjectionResolver = DependencyResolver.For(AkkaSystem);
    }

    protected ActorSystem AkkaSystem { get; }
    protected DependencyResolver DependencyInjectionResolver { get; }
    protected IServiceProvider ServiceProvider { get; }

    protected async Task GracefullyShutdownAkkaSystemAsync()
    {
      // strictly speaking this may not be necessary - terminating the ActorSystem would also work
      // but this call guarantees that the shutdown of the cluster is graceful regardless
      await CoordinatedShutdown.Get(AkkaSystem).Run(CoordinatedShutdown.ClrExitReason.Instance);
    }

    protected void RegisterApplicationShutdownIfAkkaSystemTerminates(IHostApplicationLifetime appLifetime, CancellationToken cancellationToken)
    {
      // add a continuation task that will guarantee shutdown of application if ActorSystem terminates
      AkkaSystem.WhenTerminated.ContinueWith(_ => appLifetime.StopApplication(), cancellationToken);
    }
  }
}
