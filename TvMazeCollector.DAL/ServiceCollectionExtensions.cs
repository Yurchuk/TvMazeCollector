using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TvMazeCollector.DAL;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(x => new TvMazeDbContext(configuration.GetConnectionString(nameof(TvMazeDbContext))));
        
        return services;
    }
}