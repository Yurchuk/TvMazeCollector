using static TVMazeCollector.DataCollector.HttpClients.TvMazeClient;
using System.Text.Json.Serialization;

namespace TVMazeCollector.DataCollector.Models;

public class GetActorsModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly? Birthday { get; set; }
}