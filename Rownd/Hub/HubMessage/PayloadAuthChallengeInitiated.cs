using System;
using System.Collections.Generic;
using JsonKnownTypes;
using Newtonsoft.Json;

namespace Rownd.Maui.Hub.HubMessage
{
    [JsonKnownThisType("auth_challenge_initiated")]
    public class PayloadAuthChallengeInitiated : PayloadBase
    {
        [JsonProperty("challenge_id")]
        public string ChallengeId { get; set; }

        [JsonProperty("user_identifier")]
        public string UserIdentifier { get; set; }
    }
}
