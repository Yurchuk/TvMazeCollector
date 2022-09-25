using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TVMazeCollector.DataCollector.HttpClients;
using TVMazeCollector.DataCollector.Options;
using TVMazeCollector.DataCollector.Services;


namespace TVMazeCollector.DataCollector
{
    internal class CastCollectorBackgroundService : BackgroundService
    {
        private readonly ITvMazeClient _tvMazeClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly RequestLimiterOptions _requestLimiterOptions;
        private readonly ILogger<CastCollectorBackgroundService> _logger;

        public CastCollectorBackgroundService(ITvMazeClient tvMazeClient, IServiceScopeFactory serviceScopeFactory,
            RequestLimiterOptions requestLimiterOptions, ILogger<CastCollectorBackgroundService> logger)
        {
            _tvMazeClient = tvMazeClient;
            _serviceScopeFactory = serviceScopeFactory;
            _requestLimiterOptions = requestLimiterOptions;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ICollection<int> showsIdsToGetCast;
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var showService = scope.ServiceProvider.GetRequiredService<IShowService>();
                        showsIdsToGetCast = await showService.GetCreatedShowIdsAsync(_requestLimiterOptions.RequestsLimit, cancellationToken);

                        if (!showsIdsToGetCast.Any())
                        {
                            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
                        }
                    }

                    await Parallel.ForEachAsync(showsIdsToGetCast, GetParallelOptions(cancellationToken), async (showId, cancellation) =>
                    {
                        _logger.LogInformation($"Getting cast for show - {showId}");
                        using var scope = _serviceScopeFactory.CreateScope();
                        var showService = scope.ServiceProvider.GetRequiredService<IShowService>();
                        var cast = await _tvMazeClient.GetCastAsync(showId, cancellation);
                        await showService.CreateActorsAsync(showId, cast.Select(x => x.Person), cancellation);
                        _logger.LogInformation($"Cast added for show - {showId}");
                    });
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Cast collecting process has been cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during collecting cast.");
                }
            }
        }

        private ParallelOptions GetParallelOptions(CancellationToken cancellationToken) => new ParallelOptions
        {
            MaxDegreeOfParallelism = _requestLimiterOptions.RequestsLimit,
            CancellationToken = cancellationToken
        };

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(CastCollectorBackgroundService)} is stopping.");
            await base.StopAsync(stoppingToken);
        }

        public override void Dispose()
        {
            _tvMazeClient.Dispose();
        }
    }
}