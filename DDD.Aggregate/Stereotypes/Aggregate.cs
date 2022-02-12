using System;

using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Event;
using Akka.Persistence.Fsm;
using Akka.Routing;

namespace DDD.Aggregate.Stereotypes
{
  public interface IAggregateState : PersistentFSM.IFsmState
  {
  }

  public abstract class AggregateBase<TAggregateState, TAggregate, TModel, TCommand> : PersistentFSM<TAggregateState, TModel, AggregateEventBase<TAggregate>>
    where TAggregateState : IAggregateState
    where TAggregate : PersistentFSM<TAggregateState, TModel, AggregateEventBase<TAggregate>>
    where TModel : AggregateModelBase<TAggregate, TModel>
    where TCommand : AggregateWriterCommand<TAggregate>
  {
    private readonly Guid _aggregateId;

    protected AggregateBase(Guid aggregateId)
    {
      _aggregateId = aggregateId;
    }

    public override string PersistenceId => $"Aggregate-{_aggregateId}";

    protected override sealed TModel ApplyEvent(AggregateEventBase<TAggregate> domainEvent, TModel currentData)
    {
      currentData = OnApplyEvent(domainEvent, currentData, out bool hasDataChanged);

      if (hasDataChanged)
      {
        Context.System.EventStream.Publish(new AggregateUpdateEvent<TAggregate, TModel>(_aggregateId, currentData));
      }

      return currentData;
    }

    protected abstract TModel OnApplyEvent(AggregateEventBase<TAggregate> domainEvent, TModel currentData,
      out bool hasDataChanged);
  }

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

  public abstract class AggregateModelBase<TAggregate, TModel>
  {
  }

  public abstract record AggregateEventBase<TAggregate>(Guid AggregateId);

  public record AggregateWriterCommand<TAggregate>(Guid AggregateId) : AggregateEventBase<TAggregate>(AggregateId);

  public record AggregateUpdateEvent<TAggregate, TModel>(Guid AggregateId, TModel Model) : AggregateEventBase<TAggregate>(AggregateId);

  public record AggregateReaderQuery<TAggregate>(Guid AggregateId);

  public record AggregateReaderResponse<TAggregate, TModel>(Guid AggregateId, TModel Model);

  public abstract class AggregateReaderBase<TAggregate, TModel, TQuery> : ReceiveActor
    where TModel : AggregateModelBase<TAggregate, TModel>
    where TQuery : AggregateReaderQuery<TAggregate>
  {
    private readonly Guid _aggregateId;
    private TModel _model;

    protected AggregateReaderBase(Guid aggregateId)
    {
      _aggregateId = aggregateId;

      Context.System.EventStream.Subscribe<AggregateUpdateEvent<TAggregate, TModel>>(Self);

      Receive<AggregateUpdateEvent<TAggregate, TModel>>(OnUpdate);
      Receive<AggregateReaderQuery<TAggregate>>(query => Sender.Tell(new AggregateReaderResponse<TAggregate, TModel>(_aggregateId, OnReadQuery(query, _model))));
    }

    protected virtual TModel OnReadQuery(AggregateReaderQuery<TAggregate> query, TModel currentModel)
    {
      return currentModel;
    }

    protected virtual void OnUpdate(AggregateUpdateEvent<TAggregate, TModel> @event)
    {
      _model = @event.Model;
    }

    protected override void PostStop()
    {
      Context.System.EventStream.Unsubscribe<AggregateReaderQuery<TAggregate>>(Self);
    }
  }
}
