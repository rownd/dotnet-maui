using System.Drawing;
using Foundation;
using Microsoft.Extensions.Logging;
using Rownd.Maui.Core;
using Rownd.Maui.Utils;
using UIKit;
using WebKit;

namespace Rownd.Maui.Hub
{
    partial class HubWebViewHandler
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

            var platformView = new HubWKWebView(RectangleF.Empty, config);

            userController = config.UserContentController;
            userController.AddScriptMessageHandler(new HubMessageReceiver((HubWebView)VirtualView), "rowndIosSDK");
            userController.AddScriptMessageHandler(new JsLoggingToConsole(), "rowndDeviceLogger");

            platformView.NavigationDelegate = new WebNavigationDelegate(platformView.NavigationDelegate, this);
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

            VirtualView.Focus();

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
        private class WebNavigationDelegate : WKNavigationDelegate
        {
            private readonly IWKNavigationDelegate _defaultDelegate;
            private HubWebViewHandler _handler;

            public WebNavigationDelegate(IWKNavigationDelegate defaultDelegate, HubWebViewHandler handler)
            {
                _defaultDelegate = defaultDelegate ?? throw new ArgumentNullException(nameof(defaultDelegate));
                _handler = handler;
            }

            public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
            {
                _defaultDelegate.DidFailNavigation(webView, navigation, error);
            }

            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                _defaultDelegate.DidFinishNavigation(webView, navigation);
            }

            public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
            {
                _defaultDelegate.DidStartProvisionalNavigation(webView, navigation);
            }

            public override async void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
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
                Loggers.Default.LogDebug(message.Body.ToString());
            }
        }
    }
}
