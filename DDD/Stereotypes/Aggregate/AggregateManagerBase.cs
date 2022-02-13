using System;

using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Persistence.Fsm;
using Akka.Routing;

namespace DDD.Stereotypes.Aggregate;

public abstract class AggregateManagerBase<TAggregateState, TAggregate, TAggregateReader, TModel, TCommand, TQuery> : UntypedActor
  where TAggregateState : IAggregateState
  where TAggregate : PersistentFSM<TAggregateState, TModel, AggregateEventBase<TAggregate>>
  where TAggregateReader : AggregateReaderBase<TAggregate, TModel, TQuery>
  where TModel : AggregateModelBase<TAggregate, TModel>
  where TCommand : AggregateWriterCommand<TAggregate>
  where TQuery : AggregateReaderQuery<TAggregate>
{
  private static int InitialAggregateReaderPoolSize => 1;

  protected override void OnReceive(object message)
  {
    switch (message)
    {
      case TCommand command:
        FindOrCreateAggregate(command.AggregateId).Forward(command);
        break;

      case TQuery query:
        FindOrCreateAggregateReaderPool(query.AggregateId).Forward(query);
        break;

      case Terminated msg:
        HandleAggregateTerminated(msg);
        break;
    }
  }

  private static IActorRef CreateAggregate(Guid aggregateId)
  {
    var aggregateRef = Context.ActorOf(DependencyResolver.For(Context.System).Props<TAggregate>(args: aggregateId), aggregateId.ToString());

    Context.Watch(aggregateRef);

    return aggregateRef;
  }

  private static IActorRef CreateAggregateReaderPool(Guid aggregateId)
  {
    var props = new RoundRobinPool(InitialAggregateReaderPoolSize).Props(DependencyResolver.For(Context.System).Props<TAggregateReader>(args: aggregateId));

    return Context.ActorOf(props, GetAggregateReaderName(aggregateId));
  }

  private static IActorRef FindOrCreateAggregate(Guid aggregateId)
  {
    return FindOrCreateChildActor(
      GetAggregateName(aggregateId),
      () => CreateAggregate(aggregateId));
  }

  private static IActorRef FindOrCreateAggregateReaderPool(Guid aggregateId)
  {
    return FindOrCreateChildActor(
      GetAggregateReaderName(aggregateId),
      () => CreateAggregateReaderPool(aggregateId));
  }

  private static IActorRef FindOrCreateChildActor(string childName, Func<IActorRef> createFunc)
  {
    var actorRef = Context.Child(childName);

    if (actorRef.IsNobody())
    {
      actorRef = createFunc.Invoke();
    }

    return actorRef;
  }

  private static string GetAggregateName(Guid aggregateId)
  {
    return $"Aggregate-{aggregateId}";
  }

  private static string GetAggregateReaderName(Guid aggregateId)
  {
    return $"AggregateReader-{aggregateId}";
  }

  private static void HandleAggregateTerminated(Terminated message)
  {
    Context.Unwatch(message.ActorRef);
  }
}
