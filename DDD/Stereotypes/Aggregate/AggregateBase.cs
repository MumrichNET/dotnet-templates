using System;

using Akka.Persistence.Fsm;

namespace DDD.Stereotypes.Aggregate;

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
