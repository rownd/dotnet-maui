using System;
using System.Collections.Generic;
using System.Text;
using JWT;
using JWT.Builder;
using Newtonsoft.Json;
using Rownd.Maui.Models.Repos;
using Rownd.Maui.Utils;

namespace Rownd.Maui.Models.Domain
{
    public class AuthState : StateBase
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ChallengeId { get; set; }
        public string UserIdentifier { get; set; }
        public UserType UserType { get; set; }

        [JsonIgnore]
        public bool IsAuthenticated
        {
            get { return AccessToken != null; }
        }

        [JsonIgnore]
        public bool IsNotAuthenticated
        {
            get { return AccessToken == null; }
        }

        [JsonIgnore]
        public bool IsAccessTokenValid
        {
            get
            {
                return Jwt.IsJwtValid(AccessToken);
            }
        }

        public string ToRphInitHash()
        {
            var stateRepo = StateRepo.Get();
            var data = new Dictionary<string, dynamic>
            {
                { "access_token", AccessToken },
                { "refresh_token", RefreshToken },
                { "app_id", stateRepo.Store.State.AppConfig.Id },
                { "app_user_id", stateRepo.Store.State.User.Id },
                { "reset_post_sign_in_reqs", AccessToken == null }
            };

            var json = JsonConvert.SerializeObject(data);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json), Base64FormattingOptions.None);
        }

        private string DecodeToken()
        {
            var valParams = ValidationParameters.Default;
            valParams.ValidateSignature = false;
            valParams.TimeMargin = 60;
            return JwtBuilder.Create()
                .WithValidationParameters(valParams)
                .Decode(AccessToken);
        }
    }
}
