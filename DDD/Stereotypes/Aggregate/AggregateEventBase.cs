using System;

namespace DDD.Aggregate.Stereotypes.Aggregate;

public abstract record AggregateEventBase<TAggregate>(Guid AggregateId);