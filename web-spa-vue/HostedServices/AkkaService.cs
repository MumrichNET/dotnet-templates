using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Akka.Actor;
using Akka.Configuration;
using Akka.DependencyInjection;

using DDD.Aggregate;
using DDD.Aggregate.Stereotypes;

using Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace web_spa_vue.HostedServices
{
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

  public class FormExampleReader : AggregateReaderBase<FormExampleAggregate, FormExampleModel, FormExampleQueryBase>
  {
    public FormExampleReader(Guid aggregateId) : base(aggregateId)
    {
    }
  }

  public class FormExampleModel : AggregateModelBase<FormExampleAggregate, FormExampleModel>
  {
    public string Name { get; set; }
    public string FamilyName { get; set; }
    public DateTime Birthday { get; set; }
    public IEnumerable<FormFile> Attachments { get; set; }
  }

  public class FormExampleManager : AggregateManagerBase<FormExampleStateBase, FormExampleAggregate, FormExampleReader, FormExampleModel,
    FormExampleCommandBase, FormExampleQueryBase>
  {
  }

  public class AkkaService : AkkaServiceBase, IHostedService
  {
    private readonly IServiceProvider _serviceProvider;

    public AkkaService(IServiceProvider serviceProvider) : base("akka-service", serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
      var formExampleManager = AkkaSystem.ActorOf(DependencyInjectionResolver.Props<FormExampleManager>());
      var aggreagateId = Guid.NewGuid();


      return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      throw new System.NotImplementedException();
    }
  }
}
