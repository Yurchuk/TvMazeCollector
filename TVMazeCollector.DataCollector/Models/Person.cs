using System.Text.Json.Serialization;
using static TVMazeCollector.DataCollector.HttpClients.TvMazeClient;

namespace TVMazeCollector.DataCollector.Models;

public class CastModel
{
    public PersonModel Person { get; set; }

    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly? Birthday { get; set; }
    }
}