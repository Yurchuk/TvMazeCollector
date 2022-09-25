using System.Diagnostics;
using TVMazeCollector.DataCollector.Options;

namespace TVMazeCollector.DataCollector.HttpClients;

internal class RequestsLimitDelegatingHandler : DelegatingHandler
{
    private readonly RequestLimiterOptions _requestLimiterOptions;
    private readonly SemaphoreSlim _semaphoreSlim;

    public RequestsLimitDelegatingHandler(RequestLimiterOptions requestLimiterOptions): base(new HttpClientHandler())
    {
        _requestLimiterOptions = requestLimiterOptions ?? throw new ArgumentNullException(nameof(requestLimiterOptions));
        _semaphoreSlim = new SemaphoreSlim(_requestLimiterOptions.RequestsLimit);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var result = await base.SendAsync(request, cancellationToken);

            stopWatch.Stop();
            var secondsLeftBeforeRelease = _requestLimiterOptions.TimeLimit.Seconds - stopWatch.Elapsed.Seconds;

            if (secondsLeftBeforeRelease >= 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(secondsLeftBeforeRelease + 1), cancellationToken); // add one 1 second just in case, as it takes some time from sending a request to receiving it via API
            }

            return result;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}