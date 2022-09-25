using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TvMazeCollector.DAL;
using TvMazeCollector.DAL.Entities;
using TVMazeCollector.DataCollector.Models;

namespace TVMazeCollector.DataCollector.Services;

public class ShowService : IShowService
{
    private readonly TvMazeDbContext _tvMazeDbContext;
    private readonly ILogger<ShowService> _logger;

    public ShowService(TvMazeDbContext tvMazeDbContext, ILogger<ShowService> logger)
    {
        _tvMazeDbContext = tvMazeDbContext;
        _logger = logger;
    }

    public async Task CreateShowAsync(IEnumerable<ShowModel> showModels, CancellationToken cancellationToken)
    {
        await _tvMazeDbContext.Shows.UpsertRange(showModels.Select(Map)).On(x => x.Id).RunAsync(cancellationToken);
    }

    public async Task CreateActorsAsync(int showId, IEnumerable<CastModel.PersonModel> persons, CancellationToken cancellationToken)
    {
        var actors = persons.DistinctBy(x => x.Id).Select(Map).ToList();
        var actorShows = actors.Select(c => new ActorShow { ShowId = showId, ActorId = c.Id }).ToList();

        var executionStrategy = _tvMazeDbContext.Database.CreateExecutionStrategy();

        await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _tvMazeDbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                await _tvMazeDbContext.Actors.UpsertRange(actors).On(x => x.Id).NoUpdate().RunAsync(cancellationToken);
                await _tvMazeDbContext.ActorShows.UpsertRange(actorShows).On(x => new { x.ActorId, x.ShowId }).NoUpdate().RunAsync(cancellationToken);

                var currentShow = await _tvMazeDbContext.Shows.FirstAsync(x => x.Id == showId, cancellationToken);
                currentShow.Status = ShowStatus.CastAdded;
                await _tvMazeDbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, $"Creating actors for show - {showId} has failed. Transaction has been rolled back.");
            }
        });
    }

    public async Task<ICollection<int>> GetCreatedShowIdsAsync(int takeCount, CancellationToken cancellationToken)
    {
        var shows = await _tvMazeDbContext.Shows.Where(x => x.Status == ShowStatus.Created).OrderBy(x => x.Id).Take(takeCount)
            .ToListAsync(cancellationToken);
        return shows.Select(x => x.Id).ToList();
    }

    public async Task<int> GetMaxShowIdAsync(CancellationToken cancellationToken)
    {
        if (!_tvMazeDbContext.Shows.Any())
        {
            return 0;
        }

        var maxValue = await _tvMazeDbContext.Shows.MaxAsync(x => x.Id, cancellationToken);
        return maxValue;
    }

    public async Task<PageModel<GetShowModel>> GetShowsAsync(int startRow, int endRow)
    {
        var query = _tvMazeDbContext.Shows.Where(x => x.Status == ShowStatus.CastAdded);
        var shows = await query.OrderBy(x => x.Id).Skip(startRow).Take(endRow - startRow)
            .Select(x => new GetShowModel
            {
                Id = x.Id,
                Name = x.Name,
                Cast = x.ActorShows.Select(c => c.Actor).OrderByDescending(c => c.Birthday).Select(c => new GetActorsModel
                {
                    Id = c.Id, 
                    Name = c.Name, 
                    Birthday = c.Birthday
                })
            }).ToListAsync();

        var totalItems = await query.CountAsync();

        return new PageModel<GetShowModel> { Items = shows, TotalItems = totalItems };
    }

    private static Actor Map(CastModel.PersonModel person)
    {
        return new Actor
        {
            Id = person.Id,
            Name = person.Name,
            Birthday = person.Birthday
        };
    }

    private static Show Map(ShowModel showModel)
    {
        return new Show
        {
            Id = showModel.Id,
            Name = showModel.Name,
            Status = ShowStatus.Created
        };
    }
}