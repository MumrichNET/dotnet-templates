using System;
using System.Collections.Generic;

using DDD.Stereotypes.Aggregate;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Mumrich.HelpersAndExtensions;

namespace WebSpaVue.Aggregates.FormExample;

public abstract record FormExampleStateBase(string Identifier) : IAggregateState;

public record FormExampleStateDefault() : FormExampleStateBase(nameof(FormExampleStateDefault))
{
  public static FormExampleStateDefault Instance => new();
}

public record FormExampleModelChangedEvent(Guid AggregateId, FormExampleModel Model) : AggregateEventBase<FormExampleAggregate>(AggregateId);

public abstract record FormExampleCommandBase(Guid AggregateId) : AggregateWriterCommand<FormExampleAggregate>(AggregateId);

public abstract record FormExampleQueryBase(Guid AggregateId) : AggregateReaderQuery<FormExampleAggregate>(AggregateId);

public record FormExampleSetPropertyCommand<TValue>(Guid AggregateId, string PropertyNamespace, TValue RawValue) : FormExampleCommandBase(AggregateId);

public class FormExampleAggregate : AggregateBase<FormExampleStateBase, FormExampleAggregate, FormExampleModel, FormExampleCommandBase>
{
  private readonly ILogger<FormExampleAggregate> _logger;

  public FormExampleAggregate(ILogger<FormExampleAggregate> logger, Guid aggregateId) : base(aggregateId)
  {
    _logger = logger;
    When(FormExampleStateDefault.Instance, (@event, _) => @event.FsmEvent switch
    {
      FormExampleSetPropertyCommand<string> command => Stay().Applying(command),
      _ => Stay()
    });
  }

  protected override FormExampleModel OnApplyEvent(AggregateEventBase<FormExampleAggregate> domainEvent, FormExampleModel currentData, out bool hasDataChanged)
  {
    hasDataChanged = domainEvent switch
    {
      FormExampleSetPropertyCommand<string>(_, var propertyNamespace, var rawValue) => currentData.TrySetProperty(
        propertyNamespace, rawValue, _logger),
      _ => false
    };

    return currentData;
  }
}

public class FormExampleManager : AggregateManagerBase<FormExampleStateBase, FormExampleAggregate, FormExampleReader, FormExampleModel,
  FormExampleCommandBase, FormExampleQueryBase>
{
}

public class FormExampleModel : AggregateModelBase<FormExampleAggregate, FormExampleModel>
{
  public IEnumerable<FormFile> Attachments { get; set; }
  public DateTime BirthDay { get; set; }
  public string FamilyName { get; set; }
  public string Name { get; set; }
}

public class FormExampleReader : AggregateReaderBase<FormExampleAggregate, FormExampleModel, FormExampleQueryBase>
{
  public FormExampleReader(Guid aggregateId) : base(aggregateId)
  {
  }
}
