using System;

namespace DDD.Aggregate.Stereotypes.Aggregate;

public record AggregateReaderQuery<TAggregate>(Guid AggregateId);