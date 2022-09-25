using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TVMazeCollector.DataCollector.HttpClients;
using TVMazeCollector.DataCollector.Services;

namespace TVMazeCollector.DataCollector
{
    internal class ShowCollectorBackgroundService : BackgroundService
    {
        private readonly ITvMazeClient _tvMazeClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ShowCollectorBackgroundService> _logger;

        public ShowCollectorBackgroundService(ITvMazeClient tvMazeClient, IServiceScopeFactory serviceScopeFactory, ILogger<ShowCollectorBackgroundService> logger)
        {
            _tvMazeClient = tvMazeClient;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var showService = scope.ServiceProvider.GetRequiredService<IShowService>();

                    var nextShowId = await NextShowId(showService, cancellationToken);
                    _logger.LogInformation($"Processing show - {nextShowId}");
                    var shows = await _tvMazeClient.GetShowsAsync(nextShowId, cancellationToken);
                    var maxIdFromClient = shows.Select(c => c.Id).MaxBy(c => c);
                    var maxIdFromContext = await showService.GetMaxShowIdAsync(cancellationToken);


                    if (maxIdFromClient == maxIdFromContext)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                        continue;
                    }

                    await showService.CreateShowAsync(shows, cancellationToken);
                    _logger.LogInformation($"Shows added: {shows.Count}");
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Show collecting process has been cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during collecting shows.");
                }
            }
        }

        private static async Task<int> NextShowId(IShowService showService, CancellationToken cancellationToken)
        {
            var maxId = await showService.GetMaxShowIdAsync(cancellationToken);

            if (maxId <= 0)
            {
                return 1;
            }

            return maxId + 1;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(ShowCollectorBackgroundService)} is stopping.");
            await base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _tvMazeClient.Dispose();
        }
    }
}