using System;
using System.Collections.Generic;
using GuerrillaNtp;
using JWT;
using JWT.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Rownd.Maui.Core;

namespace Rownd.Maui.Utils
{
    public class Jwt
    {
        public Jwt() { }

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

                if (jwtClaims["exp"] == null)
                {
                    return false;
                }

                // Coerce exp to long
                var jwtExp = Convert.ToInt64(jwtClaims["exp"]);
                var jwtExpTime = DateTimeOffset.FromUnixTimeSeconds(jwtExp).DateTime;
                var ntpClient = Shared.ServiceProvider.GetService<NtpClient>();
                var ntpClock = Shared.NtpClock ?? ntpClient.Last;

                // Try our best to get internet time, but fallback to local time if we must
                var currentTime = ntpClock?.UtcNow.UtcDateTime ?? DateTime.UtcNow;

                return jwtExpTime > currentTime.AddMinutes(5);
            }
            catch
            {
                return false;
            }
        }
    }
}
