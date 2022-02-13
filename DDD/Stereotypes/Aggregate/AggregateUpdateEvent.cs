using System;

namespace DDD.Stereotypes.Aggregate;

public record AggregateUpdateEvent<TAggregate, TModel>(Guid AggregateId, TModel Model) : AggregateEventBase<TAggregate>(AggregateId);
