namespace TVMazeCollector.DataCollector.Options;

public class RequestLimiterOptions
{
    public int RequestsLimit { get; set; } = 20;
    public TimeSpan TimeLimit { get; set; } = TimeSpan.FromSeconds(10);
}