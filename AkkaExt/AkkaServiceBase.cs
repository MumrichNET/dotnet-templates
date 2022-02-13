using System;

using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;

namespace AkkaExt
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
  }
}
