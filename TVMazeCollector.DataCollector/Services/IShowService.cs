using TVMazeCollector.DataCollector.Models;

namespace TVMazeCollector.DataCollector.Services;

public interface IShowService
{
    Task CreateShowAsync(IEnumerable<ShowModel> showModels, CancellationToken cancellationToken);
    Task CreateActorsAsync(int showId, IEnumerable<CastModel.PersonModel> persons, CancellationToken cancellationToken);
    Task<ICollection<int>> GetCreatedShowIdsAsync(int takeCount, CancellationToken cancellationToken);
    Task<int> GetMaxShowIdAsync(CancellationToken cancellationToken);
    Task<PageModel<GetShowModel>> GetShowsAsync(int startRow, int endRow);
}