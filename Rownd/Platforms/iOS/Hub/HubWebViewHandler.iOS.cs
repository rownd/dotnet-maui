namespace Rownd.Maui.Hub
{
    using System.Drawing;
    using CoreGraphics;
    using Foundation;
    using Microsoft.Extensions.Logging;
    using Microsoft.Maui.Handlers;
    using Rownd.Maui.Core;
    using Rownd.Maui.Utils;
    using UIKit;
    using WebKit;

    partial class HubWebViewHandler : WebViewHandler
    {
        private WKUserContentController userController;
        private bool isKeyboardStateChanging = false;

        private UIEdgeInsets ScreenInsets
        {
            get
            {
                if (DeviceInfo.Idiom != DeviceIdiom.Phone)
                {
                    return default;
                }

                var keyWindow = UIApplication.SharedApplication
                    .ConnectedScenes
                    .OfType<UIWindowScene>()
                    .SelectMany(scene => scene.Windows)
                    .FirstOrDefault(window => window.IsKeyWindow);

                if (keyWindow == null)
                {
                    Loggers.Default.LogDebug("Did not find the key window");
                    return default;
                }

                return keyWindow.SafeAreaInsets;
            }
        }

        protected override WKWebView CreatePlatformView()
        {
            var config = new WKWebViewConfiguration();

            var platformView = new HubWKWebView(RectangleF.Empty, this, config);

            userController = config.UserContentController;
            userController.AddScriptMessageHandler(new HubMessageReceiver((HubWebView)VirtualView), "rowndIosSDK");
            userController.AddScriptMessageHandler(new JsLoggingToConsole(), "rowndDeviceLogger");

            platformView.ScrollView.ScrollEnabled = false;
            platformView.ScrollView.Delegate = new ScrollDelegate(this);
            platformView.CustomUserAgent = Constants.DEFAULT_WEB_USER_AGENT;

            if (platformView.ScrollView.PinchGestureRecognizer != null)
            {
                platformView.ScrollView.PinchGestureRecognizer.Enabled = false;
            }

            var bottomPadding = ScreenInsets.Bottom == 0 ? 40 : ScreenInsets.Bottom * 2;
            ((HubWebView)VirtualView).Margin = new Thickness
            {
                Bottom = bottomPadding + ScreenInsets.Top
            };

            ListenForKeyboardNotifications();

            //// Legacy Developer Extras setting.
            var enableWebDevTools = ((HubWebView)VirtualView).EnableWebDevTools;
            config.Preferences.SetValueForKey(NSObject.FromObject(enableWebDevTools), new NSString("developerExtrasEnabled"));

            if (OperatingSystem.IsMacCatalystVersionAtLeast(major: 13, minor: 3) ||
                OperatingSystem.IsIOSVersionAtLeast(major: 16, minor: 4))
            {
                // Enable Developer Extras for Catalyst/iOS builds for 16.4+
                platformView.SetValueForKey(NSObject.FromObject(enableWebDevTools), new NSString("inspectable"));
            }

            return platformView;
        }

        protected override void ConnectHandler(WKWebView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.NavigationDelegate = new WebNavigationDelegate(platformView.NavigationDelegate, this);
        }

        protected override void DisconnectHandler(WKWebView platformView)
        {
            base.DisconnectHandler(platformView);
            platformView.NavigationDelegate = null!;
        }

        private void ListenForKeyboardNotifications()
        {
            var hubWebView = (HubWebView)VirtualView;

            // Handle keyboard showing notifications
            UIKeyboard.Notifications.ObserveWillShow((sender, args) =>
            {
                isKeyboardStateChanging = true;
                CGRect keyboardFrame = args.FrameEnd;
                _ = hubWebView.HandleKeyboardStateChange(true, 0);
            });

            UIKeyboard.Notifications.ObserveDidShow((sender, args) =>
            {
                isKeyboardStateChanging = false;
            });

            // Handle keyboard hide notifications
            UIKeyboard.Notifications.ObserveWillHide((sender, args) =>
            {
                isKeyboardStateChanging = true;
                _ = hubWebView.HandleKeyboardStateChange(false, 0);
            });

            UIKeyboard.Notifications.ObserveDidHide((sender, args) =>
            {
                isKeyboardStateChanging = false;
            });
        }

        private class ScrollDelegate : UIScrollViewDelegate
        {
            private readonly HubWebViewHandler _handler;

            public ScrollDelegate(HubWebViewHandler handler)
            {
                _handler = handler;
            }

            public override void Scrolled(UIScrollView scrollView)
            {
                if (!_handler.isKeyboardStateChanging)
                {
                    return;
                }

                if (scrollView.ContentOffset.Y != 0)
                {
                    scrollView.ContentOffset = new CoreGraphics.CGPoint
                    {
                        Y = 0,
                        X = scrollView.ContentOffset.X
                    };
                }
            }
        }

        #region WKNavigationDelegate
        private class WebNavigationDelegate : NSObject, IWKNavigationDelegate
        {
            private readonly IWKNavigationDelegate _defaultDelegate;
            private HubWebViewHandler _handler;

            public WebNavigationDelegate(IWKNavigationDelegate defaultDelegate, HubWebViewHandler handler)
            {
                _defaultDelegate = defaultDelegate ?? throw new ArgumentNullException(nameof(defaultDelegate));
                _handler = handler;
            }

            [Export("webView:didFailNavigation:withError:")]
            public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                _defaultDelegate.DidFailNavigation(webView, navigation, error);
            }

            [Export("webView:didFinishNavigation:")]
            public void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                _defaultDelegate.DidFinishNavigation(webView, navigation);
            }

            [Export("webView:didStartProvisionalNavigation:")]
            public virtual void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation) { }

            [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
            public async void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                if (navigationAction.NavigationType == WKNavigationType.LinkActivated)
                {
                    var url = navigationAction.Request.Url;
                    if (await ((HubWebView)_handler.VirtualView).HandleLinkActivationAsync(url.ToString()))
                    {
                        decisionHandler(WKNavigationActionPolicy.Allow);
                    }
                    else
                    {
                        decisionHandler(WKNavigationActionPolicy.Cancel);
                    }

                    return;
                }

                decisionHandler(WKNavigationActionPolicy.Allow);
            }
        }
        #endregion

        private class HubMessageReceiver : WKScriptMessageHandler
        {
            private readonly HubWebView hubWebView;

            public HubMessageReceiver(HubWebView hubWebView)
            {
                this.hubWebView = hubWebView;
            }

            public override void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                hubWebView.HandleHubMessage(message.Body.ToString());
            }
        }

        private class JsLoggingToConsole : WKScriptMessageHandler
        {
            public override void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                Loggers.Default.LogTrace(message.Body.ToString());
            }
        }
    }
}
