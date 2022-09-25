namespace TVMazeCollector.DataCollector.Options
{
    public class HttpClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
