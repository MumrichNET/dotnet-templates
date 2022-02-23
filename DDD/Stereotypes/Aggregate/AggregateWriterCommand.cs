using System;

namespace DDD.Stereotypes.Aggregate;

public record AggregateWriterCommand<TAggregate>(Guid AggregateId) : AggregateEventBase<TAggregate>(AggregateId);
