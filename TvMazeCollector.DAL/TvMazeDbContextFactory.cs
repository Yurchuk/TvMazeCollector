using Microsoft.EntityFrameworkCore.Design;

namespace TvMazeCollector.DAL;

public class TvMazeDbContextFactory : IDesignTimeDbContextFactory<TvMazeDbContext>
{
    public TvMazeDbContext CreateDbContext(string[] args)
    {

        if (args == null || args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            throw new ArgumentException($"{nameof(args)} must contain connection string");
        }

        var connectionString = args[0];
        return new TvMazeDbContext(connectionString);
    }
}