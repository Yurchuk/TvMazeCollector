using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using TVMazeCollector.DataCollector.HttpClients;
using TVMazeCollector.DataCollector.Options;
using TVMazeCollector.DataCollector.Services;

namespace TVMazeCollector.DataCollector.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDataCollector(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RequestLimiterOptions>(configuration);
        //var requestLimiterOptions = configuration.GetSection(nameof(ApplicationOptions.RequestLimiter)).Get<RequestLimiterOptions>();
        services.AddSingleton(x =>
            x.GetRequiredService<IConfiguration>().GetSection(nameof(ApplicationOptions.RequestLimiter)).Get<RequestLimiterOptions>());
        //services.AddTransient(_ => new RequestsLimitDelegatingHandler(requestLimiterOptions));
        services.AddTransient<RequestsLimitDelegatingHandler>();

        services.AddPolicies(configuration, nameof(ApplicationOptions.Policies))
            .AddHttpClient<ITvMazeClient, TvMazeClient, TvMazeClientOptions>(configuration, nameof(ApplicationOptions.TvMazeClient))
            .ConfigurePrimaryHttpMessageHandler<RequestsLimitDelegatingHandler>();

        services.AddTransient<IShowService, ShowService>();
        services.AddHostedService<CastCollectorBackgroundService>();
        services.AddHostedService<ShowCollectorBackgroundService>();

        return services;
    }

    public static IServiceCollection AddPolicies(this IServiceCollection services, IConfiguration configuration, string configurationSectionName)
    {
        services.Configure<PolicyOptions>(configuration);
        var policyOptions = configuration.GetSection(configurationSectionName).Get<PolicyOptions>();

        var policyRegistry = services.AddPolicyRegistry();
        policyRegistry.Add(
            PolicyName.HttpRetry,
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    policyOptions.HttpRetry.Count,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(policyOptions.HttpRetry.BackoffPower, retryAttempt))));
        policyRegistry.Add(
            PolicyName.HttpCircuitBreaker,
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: policyOptions.HttpCircuitBreaker.ExceptionsAllowedBeforeBreaking,
                    durationOfBreak: policyOptions.HttpCircuitBreaker.DurationOfBreak));

        return services;
    }

    public static IHttpClientBuilder AddHttpClient<TClient, TImplementation, TClientOptions>(
        this IServiceCollection services, IConfiguration configuration, string configurationSectionName)
        where TClient : class
        where TImplementation : class, TClient
        where TClientOptions : HttpClientOptions, new()
    {
        var httpBuilder = services.Configure<TClientOptions>(configuration.GetSection(configurationSectionName))
            .AddHttpClient<TClient, TImplementation>()
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var httpClientOptions = serviceProvider
                    .GetRequiredService<IOptions<TClientOptions>>()
                    .Value;
                httpClient.BaseAddress = httpClientOptions.BaseAddress;
                httpClient.Timeout = httpClientOptions.Timeout;
            })
            .AddPolicyHandlerFromRegistry(PolicyName.HttpRetry)
            .AddPolicyHandlerFromRegistry(PolicyName.HttpCircuitBreaker);
        
        return httpBuilder;
    }

    private static class PolicyName
    {
        public const string HttpCircuitBreaker = nameof(HttpCircuitBreaker);
        public const string HttpRetry = nameof(HttpRetry);
    }
}