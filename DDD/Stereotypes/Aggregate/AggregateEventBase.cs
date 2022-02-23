using System;

namespace DDD.Stereotypes.Aggregate;

public abstract record AggregateEventBase<TAggregate>(Guid AggregateId);
