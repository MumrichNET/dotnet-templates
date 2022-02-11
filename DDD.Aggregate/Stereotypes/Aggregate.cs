using System;

using Akka.Actor;
using Akka.Persistence;
using Akka.Persistence.Fsm;
using Akka.Routing;

namespace DDD.Aggregate.Stereotypes
{
  public interface IAggregateState : PersistentFSM.IFsmState
  {
  }

  public abstract class AggregateBase<TAggregate, TModel, TCommand> : PersistentFSM<IAggregateState, TModel, AggregateEventBase<TAggregate>>
    where TAggregate : PersistentFSM<IAggregateState, TModel, AggregateEventBase<TAggregate>>
    where TModel : AggregateModelBase<TAggregate, TModel>
    where TCommand : AggregateWriterCommand<TAggregate>
  {
    private readonly Guid _aggregateId;

    protected AggregateBase(Guid aggregateId)
    {
      _aggregateId = aggregateId;
    }

    public override string PersistenceId => $"Aggregate-{_aggregateId}";
  }

  public abstract class AggregateManagerBase<TAggregate, TAggregateReader, TModel, TCommand, TQuery> : UntypedActor
    where TAggregate : PersistentFSM<IAggregateState, TModel, AggregateEventBase<TAggregate>>
    where TAggregateReader : AggregateReaderBase<TAggregate, TModel, TQuery>
    where TModel : AggregateModelBase<TAggregate, TModel>
    where TCommand : AggregateWriterCommand<TAggregate>
    where TQuery : AggregateReaderQuery<TAggregate>
  {
    protected virtual int InitialAggregateReaderPoolSize => 1;

    protected virtual IActorRef CreateAggregate(Guid aggregateId)
    {
      var aggregateRef = Context.ActorOf(Props.Create<TAggregate>(args: aggregateId), aggregateId.ToString());

      Context.Watch(aggregateRef);

      return aggregateRef;
    }

    protected virtual IActorRef FindOrCreateAggregate(Guid aggregateId)
    {
      var aggregate = Context.Child(GetAggregateName(aggregateId));

      if (aggregate.IsNobody())
      {
        aggregate = CreateAggregate(aggregateId);
      }

      return aggregate;
    }

    protected virtual IActorRef FindOrCreateAggregateReaderPool(Guid aggregateId)
    {
      var aggregateReaderPool = Context.Child(GetAggregateReaderName(aggregateId));

      if (aggregateReaderPool.IsNobody())
      {
        aggregateReaderPool = CreateAggregateReaderPool(aggregateId);
      }

      return aggregateReaderPool;
    }

    protected virtual string GetAggregateName(Guid aggregateId)
    {
      return $"Aggregate-{aggregateId}";
    }

    protected virtual string GetAggregateReaderName(Guid aggregateId)
    {
      return $"AggregateReader-{aggregateId}";
    }

    protected virtual void HandleAggregateTerminated(Terminated message)
    {
      Context.Unwatch(message.ActorRef);
    }

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

    private IActorRef CreateAggregateReaderPool(Guid aggregateId)
    {
      var props = new RoundRobinPool(InitialAggregateReaderPoolSize).Props(Props.Create<TAggregateReader>(args: aggregateId));

      return Context.ActorOf(props, GetAggregateReaderName(aggregateId));
    }
  }

  public abstract class AggregateModelBase<TAggregate, TModel>
  {
  }

  public abstract record AggregateEventBase<TAggregate>(Guid AggregateId);

  public record AggregateWriterCommand<TAggregate>(Guid AggregateId) : AggregateEventBase<TAggregate>(AggregateId);

  public record AggregateReaderQuery<TAggregate>(Guid AggregateId);

  public abstract class AggregateReaderBase<TAggregate, TModel, TQuery> : PersistentActor
    where TModel : AggregateModelBase<TAggregate, TModel>
    where TQuery : AggregateReaderQuery<TAggregate>
  {
    private readonly Guid _aggregateId;

    protected AggregateReaderBase(Guid aggregateId)
    {
      _aggregateId = aggregateId;
    }

    public override string PersistenceId => $"AggregateReader-{_aggregateId}";
  }
}
