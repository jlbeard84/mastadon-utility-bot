using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MastodonBot.Interfaces;

namespace MastodonBot.Workers
{
    public class Bot: BackgroundService
    {
        private readonly ILogger<Bot> _logger;
        private readonly IHost _host;
        private readonly ITootService _tootService;

        public Bot(
            ILogger<Bot> logger,
            IHost host,
            ITootService tootService)
        {
            _logger = logger;
            _host = host;
            _tootService = tootService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting {nameof(Bot)}.{nameof(ExecuteAsync)}");
            await _tootService.Execute(cancellationToken);
            _logger.LogInformation($"Ending {nameof(Bot)}.{nameof(ExecuteAsync)}");
        }
    }
}