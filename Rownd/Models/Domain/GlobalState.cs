using System;
using Newtonsoft.Json;

namespace Rownd.Maui.Models.Domain
{
    public class GlobalState
    {
        [JsonIgnore]
        public bool IsInitialized { get; internal set; } = false;

        [JsonIgnore]
        public bool IsReady { get; internal set; } = false;

        public AppState AppConfig { get; set; } = new AppState();
        public AuthState Auth { get; set; } = new AuthState();
        public UserState User { get; set; } = new UserState();
        public SignInState SignIn { get; set; } = new SignInState();
    }
}
