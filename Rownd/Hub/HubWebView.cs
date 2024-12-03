using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rownd.Controls;
using Rownd.Maui.Core;
using Rownd.Maui.Hub.HubMessage;
using Rownd.Maui.Models;
using Rownd.Maui.Models.Domain;
using Rownd.Maui.Models.Repos;
using Rownd.Maui.Utils;

namespace Rownd.Maui.Hub
{
    internal partial class HubWebView : WebView, IBottomSheetChild
    {
        private readonly Config config = Shared.ServiceProvider.GetService<Config>();
        private readonly StateRepo stateRepo = StateRepo.Get();
        private HubBottomSheetPage bottomSheet { get; set; }
        internal double KeyboardHeight { get; private set; } = 0;
        internal RowndSignInJsOptions HubOpts { get; set; } = new RowndSignInJsOptions();
        internal HubPageSelector TargetPage { get; set; } = HubPageSelector.SignIn;
        internal bool EnableWebDevTools = true;

        private partial Task InitializeWebView();

        private partial void NavigateCore(string url);

#if !ANDROID && !IOS && !MACCATALYST && !WINDOWS
        private partial Task InitializeWebView() => throw null!;
        private partial void NavigateCore(string url) => throw null!;
#endif

        public HubWebView()
        {
            Navigated += OnPageLoaded;
            Navigating += WebView_Navigating;

            this.FadeTo(0, 0);
        }

        public HubWebView(HubPageSelector page, RowndSignInJsOptions opts) : this()
        {
            HubOpts = opts ?? new RowndSignInJsOptions();
            TargetPage = page;
        }

        protected override async void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            await InitializeWebView();
        }

        internal async void RenderHub()
        {
            var url = await config.GetHubLoaderUrl();
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var connectionState = Connectivity.NetworkAccess;
                if (connectionState != NetworkAccess.Internet)
                {
                    Source = new HtmlWebViewSource
                    {
                        Html = NoInternet.Build(Shared.Rownd)
                    };
                    await this.FadeTo(1, 500);
                    bottomSheet.IsLoading = false;
                    await bottomSheet.RequestHeight(400);
                    return;
                }

                if (Source == null)
                {
                    NavigateCore(url);
                }
                else if (Source is UrlWebViewSource source && source.Url != url)
                {
                    NavigateCore(url);
                    TriggerHub();
                }
                else
                {
                    TriggerHub();
                }
            });
        }

        public void TriggerHub()
        {
            SetFeatureFlagsJS();

            switch (TargetPage)
            {
                case HubPageSelector.SignIn:
                    {
                        EvaluateJavaScript($"rownd.requestSignIn({HubOpts?.ToJsonString()});");
                        break;
                    }

                case HubPageSelector.SignOut:
                    {
                        EvaluateJavaScript("rownd.signOut({\"show_success\":true});");
                        break;
                    }

                case HubPageSelector.Profile:
                    {
                        EvaluateJavaScript($"rownd.user.manageAccount({HubOpts?.ToJsonString()});");
                        break;
                    }

                case HubPageSelector.ConnectAuthenticator:
                    {
                        EvaluateJavaScript($"rownd.connectAuthenticator({HubOpts?.ToJsonString()})");
                        break;
                    }

                case HubPageSelector.None:
                    {
                        break;
                    }
            }
        }

        protected void EvaluateJavaScript(string code)
        {
            Loggers.Default.LogTrace("Executing JS: {code}", code);
            var wrappedJs = $@"
if (typeof rownd !== 'undefined') {{
    {code}
}} else {{
    _rphConfig.push(['onLoaded', () => {{
        {code}
    }}]);
}}
".Replace(System.Environment.NewLine, "");

            EvaluateJavaScriptAsync(wrappedJs);
        }

        public void HandleHubMessage(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Loggers.Default.LogDebug("Received message: {message}", message);
                try
                {
                    var hubMessage = JsonConvert.DeserializeObject<Message>(message, new JsonConverterPayload());

                    switch (hubMessage?.Type)
                    {
                        case MessageType.Authentication:
                            {
                                Loggers.HubWebView.LogDebug($"Received auth payload: {hubMessage.Payload}");
                                stateRepo.Store.Dispatch(new StateActions.SetAuthState
                                {
                                    AuthState = new AuthState()
                                    {
                                        AccessToken = (hubMessage.Payload as PayloadAuthenticated).AccessToken,
                                        RefreshToken = (hubMessage.Payload as PayloadAuthenticated).RefreshToken
                                    }
                                });

                                await UserRepo.GetInstance().FetchUser();

                                // Reset last sign in state
                                stateRepo.Store.Dispatch(new StateActions.SetSignInState { SignInState = new SignInState() });
                                break;
                            }

                        case MessageType.HubLoaded:
                            {
                                await this.FadeTo(1, 500);
                                bottomSheet.IsLoading = false;
                                break;
                            }

                        case MessageType.HubResize:
                            {
                                Loggers.Default.LogTrace("Hub resize request: {payload}", hubMessage.Payload);
                                await bottomSheet.RequestHeight((hubMessage.Payload as PayloadHubResize).Height);
                                break;
                            }

                        case MessageType.CanTouchBackgroundToDismiss:
                            {
                                Loggers.Default.LogTrace("Hub dismissable change: {payload}", hubMessage.Payload);
                                bottomSheet.IsDismissable = (hubMessage.Payload as PayloadCanTouchBackgroundToDismiss).Enable;
                                break;
                            }

                        case MessageType.CloseHub:
                            {
                                Loggers.Default.LogTrace("Hub close request");
                                bottomSheet.IsDismissable = true;
                                _ = bottomSheet.Dismiss();
                                break;
                            }

                        case MessageType.UserDataUpdate:
                            {
                                Loggers.Default.LogTrace("User data received: {payload}", hubMessage.Payload);
                                stateRepo.Store.Dispatch(new StateActions.SetUserState
                                {
                                    UserState = new UserState()
                                    {
                                        Data = (hubMessage.Payload as PayloadUserDataUpdate).Data
                                    }
                                });
                                Shared.Rownd.FireEvent(new PayloadEvent
                                {
                                    Event = "user_data_update",
                                    Data = (hubMessage.Payload as PayloadUserDataUpdate).Data
                                });
                                break;
                            }

                        case MessageType.TriggerSignInWithApple:
                            {
                                Shared.Rownd.RequestSignIn(SignInMethod.Apple);
                                break;
                            }

                        case MessageType.Event:
                            {
                                Shared.Rownd.FireEvent((PayloadEvent)hubMessage.Payload);
                                break;
                            }

                        case MessageType.TryAgain:
                            {
                                Source = null;
                                RenderHub();
                                break;
                            }

                        case MessageType.AuthChallengeInitiated:
                            {
                                var payload = hubMessage.Payload as PayloadAuthChallengeInitiated;
                                stateRepo.Store.Dispatch(new StateActions.SetAuthState
                                {
                                    AuthState = new AuthState
                                    {
                                        ChallengeId = payload.ChallengeId,
                                        UserIdentifier = payload.UserIdentifier
                                    }
                                });
                                break;
                            }

                        case MessageType.AuthChallengeCleared:
                            {
                                stateRepo.Store.Dispatch(new StateActions.SetAuthState
                                {
                                    AuthState = new AuthState
                                    {
                                        AccessToken = stateRepo.Store.State.Auth.AccessToken,
                                        RefreshToken = stateRepo.Store.State.Auth.RefreshToken,
                                        UserType = stateRepo.Store.State.Auth.UserType,
                                    }
                                });
                                break;
                            }

                        default:
                            {
                                Loggers.HubWebView.LogDebug($"No handler for message type '{hubMessage?.Type}'.");
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Loggers.HubWebView.LogError(e, $"Failed to decode hub message ({message})");
                }
            });
        }

        public async Task HandleKeyboardStateChange(bool isKeyboardOpen, double keyboardHeight)
        {
            KeyboardHeight = keyboardHeight;
            if (!isKeyboardOpen)
            {
                await bottomSheet.RequestHeight(bottomSheet.LastRequestedPosition);
                return;
            }

            await bottomSheet.Expand();
        }

        public void OnPageLoaded(object sender, WebNavigatedEventArgs e)
        {
            if (e.Url.StartsWith(config.HubUrl))
            {
                TriggerHub();
            }
        }

        public void SetBottomSheetParent(HubBottomSheetPage bottomSheet)
        {
            this.bottomSheet = bottomSheet;
        }

        public async Task<bool> HandleLinkActivationAsync(string linkUrl)
        {
            // Load only Rownd-related URLs in the webview
            string[] allowedWebViewUrls =
            {
                "https://appleid.apple.com/auth/authorize",
                config.HubUrl,
                "about:"
            };

            foreach (string url in allowedWebViewUrls)
            {
                if (linkUrl.StartsWith(url))
                {
                    return true;
                }
            }

            await Dispatcher.DispatchAsync(async () =>
            {
                await Launcher.OpenAsync(new Uri(linkUrl));
            });

            return false;
        }

        public async void WebView_Navigating(object sender, WebNavigatingEventArgs args)
        {
            // iOS WKWebView uses a special handler in its custom renderer
            // due to behavioral differences
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                return;
            }

            if (await HandleLinkActivationAsync(args.Url))
            {
                return;
            }

            args.Cancel = true;
        }

        private void SetFeatureFlagsJS()
        {
            var supportedFeaturesStr = Constants.GetSupportedFeatures();
            var code = @$"
if (rownd?.setSessionStorage) {{
    rownd.setSessionStorage('rph_feature_flags',`{supportedFeaturesStr}`);
}}
";
            EvaluateJavaScript(code);
        }
    }
}