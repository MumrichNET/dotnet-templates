using Akka.Actor;
using Akka.DependencyInjection;

namespace Mumrich.AkkaExt.Extensions;

public static class ActorSystemExtensions
{
  public static IActorRef ActorOfWithNameAndArgs<T>(
    this ActorSystem actorSystem,
    string name,
    params object[] args) where T : ActorBase
  {
    var diResolver = DependencyResolver.For(actorSystem);

    return actorSystem.ActorOf(diResolver.Props<T>(args), name);
  }

  public static IActorRef ActorOfWithArgs<T>(
    this ActorSystem actorSystem,
    params object[] args) where T : ActorBase
  {
    return actorSystem.ActorOfWithNameAndArgs<T>(name: null, args);
  }
}