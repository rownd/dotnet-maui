namespace Rownd.Maui.Models
{
    using ReduxSimple;
    using Rownd.Maui.Core;
    using Rownd.Maui.Models.Domain;
    using Rownd.Maui.Utils;
    using static ReduxSimple.Reducers;

    public static class StateReducers
    {
        public static IEnumerable<On<GlobalState>> CreateReducers()
        {
            return CombineReducers(
                GetAppConfigReducers(),
                GetAuthReducers(),
                GetUserReducers(),
                GetSignInStateReducers()
            );
        }

        public static IEnumerable<On<GlobalState>> GetAppConfigReducers()
        {
            return CreateSubReducers(StateSelectors.SelectAppConfigState)
                .On<StateActions.SetAppConfig>(
                    (state, action) =>
                    {
                        return action.AppConfig;
                    }
            ).ToList();
        }

        public static IEnumerable<On<GlobalState>> GetAuthReducers()
        {
            return CreateSubReducers(StateSelectors.SelectAuthState)
                .On<StateActions.SetAuthState>(
                    (state, action) =>
                    {
                        return action.AuthState;
                    }
            ).ToList();
        }

        public static IEnumerable<On<GlobalState>> GetUserReducers()
        {
            return CreateSubReducers(StateSelectors.SelectUserState)
                .On<StateActions.SetUserState>(
                    (state, action) =>
                    {
                        return action.UserState;
                    }
            ).ToList();
        }

        public static IEnumerable<On<GlobalState>> GetSignInStateReducers()
        {
            return CreateSubReducers(StateSelectors.SelectSignInState)
                .On<StateActions.SetSignInState>(
                    (state, action) =>
                    {
                        if (action.SignInState.LastSignIn != null)
                        {
                            var timeManager = Shared.TimeManager;
                            var currentTime = timeManager.CurrentTime ?? DateTime.UtcNow;
                            action.SignInState.LastSignInDate = currentTime.ToUniversalISO8601();
                        }

                        return action.SignInState;
                    })
                .ToList();
        }
    }
}
