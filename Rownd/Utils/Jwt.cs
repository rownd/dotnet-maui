using JWT.Serializers;

namespace Rownd.Maui.Utils
{
    using JWT;
    using JWT.Builder;
    using Newtonsoft.Json;
    using Rownd.Maui.Core;

    public class Jwt
    {
        private static readonly NewtonsoftJsonSerializer Serializer = new();

        public static bool IsJwtValid(string token)
        {
            try
            {
                var valParams = ValidationParameters.Default;
                valParams.ValidateSignature = false;
                valParams.TimeMargin = 60;
                var json = JwtBuilder.Create()
                    .WithJsonSerializer(Serializer)
                    .WithValidationParameters(valParams)
                    .Decode(token);

                // Get claims as dictionary
                var jwtClaims = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                if (jwtClaims?["exp"] == null)
                {
                    return false;
                }

                // Coerce exp to long
                var jwtExp = Convert.ToInt64(jwtClaims["exp"]);
                var jwtExpTime = DateTimeOffset.FromUnixTimeSeconds(jwtExp).UtcDateTime;
                var timeManager = Shared.TimeManager;

                // Try our best to get internet time, but fallback to local time if we must
                var currentTime = timeManager?.CurrentTime ?? DateTime.UtcNow;

                return jwtExpTime > currentTime.AddMinutes(5);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private class NewtonsoftJsonSerializer(JsonSerializerSettings? settings = null) : IJsonSerializer
        {
            private readonly JsonSerializerSettings settings = settings ?? new JsonSerializerSettings();

            public string Serialize(object obj)
            {
                return JsonConvert.SerializeObject(obj, this.settings);
            }

            public object? Deserialize(Type type, string json)
            {
                return JsonConvert.DeserializeObject(json, type, this.settings);
            }
        }
    }
}
