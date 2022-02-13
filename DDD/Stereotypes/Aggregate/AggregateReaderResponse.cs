using System;

namespace DDD.Aggregate.Stereotypes.Aggregate;

public record AggregateReaderResponse<TAggregate, TModel>(Guid AggregateId, TModel Model);