namespace Rownd.Maui.Utils
{
    using JWT;
    using JWT.Builder;
    using Newtonsoft.Json;
    using Rownd.Maui.Core;

    public class Jwt
    {
        public static bool IsJwtValid(string token)
        {
            try
            {
                var valParams = ValidationParameters.Default;
                valParams.ValidateSignature = false;
                valParams.TimeMargin = 60;
                var json = JwtBuilder.Create()
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
                var jwtExpTime = DateTimeOffset.FromUnixTimeSeconds(jwtExp).DateTime;
                var timeManager = Shared.TimeManager;

                // Try our best to get internet time, but fallback to local time if we must
                var currentTime = timeManager?.CurrentTime ?? DateTime.UtcNow;

                return jwtExpTime > currentTime.AddMinutes(5);
            }
            catch
            {
                return false;
            }
        }
    }
}
