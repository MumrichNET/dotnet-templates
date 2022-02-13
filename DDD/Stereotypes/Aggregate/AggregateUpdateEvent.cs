using System;

namespace DDD.Aggregate.Stereotypes.Aggregate;

public record AggregateUpdateEvent<TAggregate, TModel>(Guid AggregateId, TModel Model) : AggregateEventBase<TAggregate>(AggregateId);