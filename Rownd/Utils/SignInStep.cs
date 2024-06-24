using System;
using Newtonsoft.Json;

namespace Rownd.Maui.Utils
{
    public enum SignInStep
    {
        [JsonProperty("init")]
        Init,

        [JsonProperty("no_account")]
        NoAccount,

        [JsonProperty("success")]
        Success,

        [JsonProperty("completing")]
        Completing,

        [JsonProperty("error")]
        Error,
    }
}
