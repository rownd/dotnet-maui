using HttpTracer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using Rownd.Maui.Core;

namespace Rownd.Maui.Utils;

public class TimeManager
{
    internal static TimeManager shared = new TimeManager();

    private RestClient Client { get; set; }

    private DateTime? FetchedWorldTime { get; set; }

    private DateTime? FetchTime { get; set; }

    internal DateTime? CurrentTime
    {
        get
        {
            if (FetchedWorldTime == null || FetchTime == null)
            {
                Loggers.Default.LogWarning("Network time not available");
                return null;
            }

            var timePassed = default(DateTime).Subtract(FetchTime.Value);

            return FetchedWorldTime.Value.Add(timePassed);
        }
    }

    private class TimeData
    {
        [JsonProperty("utc_datetime")]
        public required string UtcDateTime { get; set; }
    }

    private TimeManager()
    {
        var options = new RestClientOptions("https://worldtimeapi.org")
        {
            ConfigureMessageHandler = handler => new HttpTracerHandler(handler, Loggers.Http, HttpMessageParts.All),
            UserAgent = Constants.DEFAULT_API_USER_AGENT,
        };

        JsonSerializerSettings defaultSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            },
            DefaultValueHandling = DefaultValueHandling.Include,
            TypeNameHandling = TypeNameHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        };

        this.Client = new RestClient(
            options,
            configureSerialization: s => s.UseNewtonsoftJson(defaultSettings)
        );

        Task.Run(async () =>
        {
            await this.GetCurrentTimeAsync();
        });
    }

    private async Task GetCurrentTimeAsync()
    {
        try
        {
            var resp = await this.Client.GetAsync<TimeData>(new RestRequest()
            {
                Resource = "/api/timezone/Etc/UTC",
            });

            if (resp == null)
            {
                throw new Exception("Get time data was null");
            }

            DateTime fetchedDate = DateTime.Parse(resp.UtcDateTime);

            this.FetchTime = default(DateTime);
            this.FetchedWorldTime = fetchedDate;

            Loggers.Default.LogDebug("Network time fetched: {fetchedDate}", fetchedDate);
        }
        catch (Exception ex)
        {
            Loggers.Default.LogWarning("Error fetching network time: {message}", ex.Message);
        }
    }

    private void Dispose()
    {
        this.Client.Dispose();
    }
}