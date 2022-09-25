using Microsoft.EntityFrameworkCore;
using TvMazeCollector.DAL.Entities;

namespace TvMazeCollector.DAL;

public class TvMazeDbContext : DbContext
{
    private readonly string _connectionString;

    public TvMazeDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString, options =>
        {
            options.EnableRetryOnFailure();
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TvMazeDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
        builder.Properties<DateOnly>().HaveConversion<DateOnlyConverter>().HaveColumnType("date");
    }

    public DbSet<Actor> Actors { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<ActorShow> ActorShows { get; set; }
}