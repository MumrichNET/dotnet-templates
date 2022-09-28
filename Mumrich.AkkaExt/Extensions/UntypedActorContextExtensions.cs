using Akka.Actor;
using Akka.DependencyInjection;

namespace Mumrich.AkkaExt.Extensions;

public static class UntypedActorContextExtensions
{
  public static IActorRef ActorOfWithArgs<T>(
    this IUntypedActorContext untypedActorContext,
    params object[] args) where T : ActorBase
  {
    return untypedActorContext.ActorOfWithNameAndArgs<T>(null, args);
  }

  public static IActorRef ActorOfWithNameAndArgs<T>(
      this IUntypedActorContext untypedActorContext,
    string name,
    params object[] args) where T : ActorBase
  {
    var diResolver = DependencyResolver.For(untypedActorContext.System);

    return untypedActorContext.ActorOf(diResolver.Props<T>(args), name);
  }
}