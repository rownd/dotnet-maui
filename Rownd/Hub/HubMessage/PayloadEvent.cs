using System;
using System.Collections.Generic;
using JsonKnownTypes;
using Newtonsoft.Json;

namespace Rownd.Maui.Hub.HubMessage
{
    [JsonKnownThisType("event")]
    public class PayloadEvent : PayloadBase
    {
        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("data")]
        public Dictionary<string, dynamic> Data { get; set; } = new Dictionary<string, dynamic>();
    }
}
