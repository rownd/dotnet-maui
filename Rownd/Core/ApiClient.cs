using HttpTracer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Rownd.Maui.Core
{
    public class ApiClient
    {
        public RestClient Client { get; private set; }

        public static ApiClient Get()
        {
            return Shared.ServiceProvider.GetRequiredService<ApiClient>();
        }

        public ApiClient()
        {
            var config = Config.GetConfig();
            var options = new RestClientOptions(config.ApiUrl)
            {
                Authenticator = new ApiAuthenticator(),
                ConfigureMessageHandler = handler => new HttpTracerHandler(handler),
                UserAgent = Constants.DEFAULT_API_USER_AGENT,
            };

            JsonSerializerSettings defaultSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                DefaultValueHandling = DefaultValueHandling.Include,
                TypeNameHandling = TypeNameHandling.None,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };

            Client = new RestClient(
                options,
                configureSerialization: s => s.UseNewtonsoftJson(defaultSettings)
            );

            Client.AddDefaultHeader("x-rownd-app-key", config.AppKey);
        }
    }
}
