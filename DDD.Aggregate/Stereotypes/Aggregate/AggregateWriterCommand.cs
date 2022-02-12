using System;

namespace DDD.Aggregate.Stereotypes.Aggregate;

public record AggregateWriterCommand<TAggregate>(Guid AggregateId) : AggregateEventBase<TAggregate>(AggregateId);