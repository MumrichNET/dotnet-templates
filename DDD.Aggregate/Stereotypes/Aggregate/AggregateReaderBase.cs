using System;

using Akka.Actor;
using Akka.Event;

namespace DDD.Aggregate.Stereotypes.Aggregate;

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