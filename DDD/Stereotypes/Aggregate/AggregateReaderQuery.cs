using System;

namespace DDD.Stereotypes.Aggregate;

public record AggregateReaderQuery<TAggregate>(Guid AggregateId);
