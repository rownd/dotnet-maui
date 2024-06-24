using ReduxSimple;
using Rownd.Controls;
using Rownd.Maui.Core;
using Rownd.Maui.Hub;
using Rownd.Maui.Hub.HubMessage;
using Rownd.Maui.Models;
using Rownd.Maui.Models.Domain;
using Rownd.Maui.Models.Repos;
using Rownd.Maui.Utils;

namespace Rownd.Maui
{
    public class RowndInstance : IRowndInstance
    {
        private HubBottomSheetPage? hubBottomSheet = null;
        private static RowndInstance inst;

        internal StateRepo State { get; private set; }
        internal AuthRepo Auth { get; private set; }

        public UserRepo User { get; private set; }

        public Config Config { get; set; }
        public ReduxStore<GlobalState> Store
        {
            get
            {
                return State.Store;
            }
        }

        public event EventHandler<RowndEventArgs> Events;

        private RowndInstance(Application app, Config? config = null)
        {
            inst = this;
            Shared.Init(this, app, config);
            Config = Shared.ServiceProvider.GetService<Config>();
            State = StateRepo.Get();
            Auth = AuthRepo.Get();
            User = UserRepo.GetInstance();
            State.Setup();

            // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                DependencyService.Get<IAppleAuthCoordinator>().Inject(this, Auth);
            }
        }

        public static RowndInstance GetInstance(Application app, Config config = null)
        {
            inst ??= new RowndInstance(app, config);

            return inst;
        }

        public RowndInstance Configure(string appKey)
        {
            var config = Shared.ServiceProvider.GetService<Config>();

            config.AppKey = appKey;

            return inst;
        }

        public void RequestSignIn()
        {
            DisplayHub(HubPageSelector.SignIn);
        }

        public void RequestSignIn(SignInMethod with)
        {
            switch (with)
            {
                case SignInMethod.Apple:
                    Console.WriteLine("RequestSignIn(.apple)");
                    var appleAuthCoord = DependencyService.Get<IAppleAuthCoordinator>();

                    Console.WriteLine("RequestSignIn(.apple): call .SignIn()");
                    appleAuthCoord.SignIn();
                    break;

                default:
                    RequestSignIn(new SignInOptions());
                    break;
            }
        }

        public void RequestSignIn(SignInOptions opts)
        {

        }

        public void RequestSignIn(RowndSignInJsOptions opts)
        {
            DisplayHub(HubPageSelector.SignIn, opts);
        }

        public void SignOut()
        {
            Store.Dispatch(new StateActions.SetAuthState() { AuthState = new AuthState() });
            Store.Dispatch(new StateActions.SetUserState() { UserState = new UserState() });
        }

        public void ManageAccount()
        {
            ManageAccount(null);
        }

        public void ManageAccount(RowndManageAccountOpts? opts)
        {
            Task.Run(async () =>
            {
                await GetAccessToken();
                var hubOpts = opts != null ? new RowndSignInJsOptions
                {
                    VisibleProfileFields = opts.VisibleProfileFields,
                    AutoFocusField = opts.AutoFocusField
                } : null;
                DisplayHub(HubPageSelector.Profile, hubOpts);
            });
        }

        public async Task<string> GetAccessToken()
        {
            return await Auth.GetAccessToken();
        }

        public async Task<string> GetAccessToken(string token)
        {
            var tokens = await Auth.GetAccessToken(token);
            return tokens.AccessToken;
        }

        public async Task<string> GetAccessToken(RowndTokenOpts opts)
        {
            var authState = await Auth.RefreshToken();
            return authState.AccessToken;
        }

        [Obsolete("Use GetAccessToken(opts) instead")]
        public async Task _InternalTestRefreshToken()
        {
            await Task.WhenAll(
                Auth.RefreshToken(),
                Auth.RefreshToken(),
                Auth.RefreshToken()
            );
        }

        #region Internal methods
        internal async void DisplayHub(HubPageSelector page, RowndSignInJsOptions? opts = null)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                // If the modal stack already includes a bottom sheet page, don't show another
                if (Application.Current?.MainPage?.Navigation.ModalStack.FirstOrDefault(el => el is HubBottomSheetPage) != null)
                {
                    return;
                }

                if (hubBottomSheet == null)
                {
                    hubBottomSheet = new HubBottomSheetPage();
                    hubBottomSheet.OnDismiss += (object sender, EventArgs e) =>
                    {
                        hubBottomSheet = null;
                    };
                }

                await Application.Current.MainPage.Navigation.PushModalAsync(hubBottomSheet, false);

                var webView = hubBottomSheet.GetHubWebView();
                webView.TargetPage = page;
                webView.HubOpts = opts ?? new RowndSignInJsOptions();
                webView.RenderHub();
            });
        }

        internal bool IsHubOpen()
        {
            return hubBottomSheet != null;
        }

        internal void FireEvent(PayloadEvent payload)
        {
            var evt = new RowndEventArgs
            {
                Event = payload.Event,
                Data = payload.Data
            };

            Events?.Invoke(this, evt);
        }

        public class RowndEventArgs : EventArgs
        {
            public string Event { get; set; }
            public Dictionary<string, dynamic> Data { get; set; }
        }

        #endregion
    }
}
