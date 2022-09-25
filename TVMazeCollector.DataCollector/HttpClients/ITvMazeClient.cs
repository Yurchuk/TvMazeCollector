using TVMazeCollector.DataCollector.Models;

namespace TVMazeCollector.DataCollector.HttpClients;

public interface ITvMazeClient: IDisposable
{
    Task<ICollection<ShowModel>> GetShowsAsync(int showNumber, CancellationToken cancellationToken);
    Task<ICollection<CastModel>> GetCastAsync(int showId, CancellationToken cancellationToken);
}