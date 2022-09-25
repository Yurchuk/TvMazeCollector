using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TVMazeCollector.DataCollector.Models;

namespace TVMazeCollector.DataCollector.HttpClients;

public class TvMazeClient : ITvMazeClient, IDisposable
{
    private const int ShowItemsPerPage = 250;
    private readonly HttpClient _httpClient;

    public TvMazeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ICollection<ShowModel>> GetShowsAsync(int showNumber, CancellationToken cancellationToken)
    {
        if (showNumber <= 0)
        {
            throw new ArgumentException($"{nameof(showNumber)} cannot be less than or equal to 0");
        }

        var page = showNumber / ShowItemsPerPage;

        var response = await _httpClient.GetAsync($"shows?page={page}", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ICollection<ShowModel>>(cancellationToken: cancellationToken);
        return result ?? new List<ShowModel>();
    }

    public async Task<ICollection<CastModel>> GetCastAsync(int showId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"shows/{showId}/cast", cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ICollection<CastModel>>(cancellationToken: cancellationToken);
        return result ?? new List<CastModel>();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();

            if (string.IsNullOrEmpty(str))
            {
                return DateOnly.MinValue;;
            }

            return DateOnly.ParseExact(str, Format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format, CultureInfo.InvariantCulture));
        }
    }
}