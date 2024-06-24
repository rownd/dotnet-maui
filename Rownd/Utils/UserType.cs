using Newtonsoft.Json;

namespace Rownd.Maui.Utils
{
    public enum UserType
    {
        [JsonProperty("new_user")]
        NewUser,

        [JsonProperty("existing_user")]
        ExistingUser,
    }
}
