namespace TVMazeCollector.DataCollector.Models;

public class GetShowModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<GetActorsModel> Cast { get; set; }
}