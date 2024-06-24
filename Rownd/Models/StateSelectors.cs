using ReduxSimple;
using Rownd.Maui.Models.Domain;
using static ReduxSimple.Selectors;

namespace Rownd.Maui.Models
{
    public static class StateSelectors
    {
        public static ISelectorWithoutProps<GlobalState, AppState> SelectAppConfigState { get; } = CreateSelector(
            (GlobalState state) => state.AppConfig
        );

        public static ISelectorWithoutProps<GlobalState, AuthState> SelectAuthState { get; } = CreateSelector(
            (GlobalState state) => state.Auth
        );

        public static ISelectorWithoutProps<GlobalState, UserState> SelectUserState { get; } = CreateSelector(
            (GlobalState state) => state.User
        );

        public static ISelectorWithoutProps<GlobalState, SignInState> SelectSignInState { get; } = CreateSelector(
            (GlobalState state) => state.SignIn
        );
    }
}
