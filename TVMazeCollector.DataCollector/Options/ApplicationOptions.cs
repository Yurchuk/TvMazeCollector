namespace TVMazeCollector.DataCollector.Options
{
    public class ApplicationOptions
    {
        public PolicyOptions Policies { get; set; }
        public TvMazeClientOptions TvMazeClient { get; set; }
        public RequestLimiterOptions RequestLimiter { get; set; }
    }
}
