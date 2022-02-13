using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SpaDevServer.Contracts;

namespace SpaDevServer.HostedServices
{
  public class SpaDevelopmentService : IHostedService
  {
    private readonly ILogger<SpaDevelopmentService> _logger; // This is a development-time only feature, so a very long timeout is fine
    private readonly ISpaDevServerSettings _spaDevServerSettings;

    public SpaDevelopmentService(ILogger<SpaDevelopmentService> logger, ISpaDevServerSettings spaDevServerSettings)
    {
      _logger = logger;
      _spaDevServerSettings = spaDevServerSettings;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
}
