using System;
using Newtonsoft.Json;

namespace Rownd.Maui.Utils
{
    public class SupportedFeature
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("enabled")]
        public string Enabled { get; set; }
    }
}
