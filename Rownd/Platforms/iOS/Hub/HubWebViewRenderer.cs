//using System;
//using System.ComponentModel.Design;
//using System.Drawing;
//using System.Linq;
//using Foundation;
//using Rownd.Maui.Core;
//using Rownd.Maui.Hub;
//using Rownd.Maui.iOS.Hub;
//using UIKit;
//using WebKit;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(HubWebView), typeof(HubWebViewRenderer))]
//namespace Rownd.Maui.iOS.Hub
//{
//    public class HubWebViewRenderer : WkWebViewRenderer, IWKScriptMessageHandler
//    {
//        private WKUserContentController userController;
//        protected bool isKeyboardStateChanging = false;

//        public override UIView InputAccessoryView
//        {
//            get
//            {
//                return null;
//            }
//        }

//        private UIEdgeInsets ScreenInsets
//        {
//            get
//            {
//                if (Device.Idiom != TargetIdiom.Phone)
//                {
//                    return default;
//                }

//                var keyWindow = UIApplication.SharedApplication
//                    .ConnectedScenes
//                    .OfType<UIWindowScene>()
//                    .SelectMany(scene => scene.Windows)
//                    .FirstOrDefault(window => window.IsKeyWindow);

//                if (keyWindow == null)
//                {
//                    Console.WriteLine("Did not find the key window");
//                    return default;
//                }

//                return keyWindow.SafeAreaInsets;
//            }
//        }

//        public HubWebViewRenderer() : this(new WKWebViewConfiguration())
//        {
//            CustomUserAgent = Constants.DEFAULT_WEB_USER_AGENT;

//            // Handle keyboard showing notifications
//            UIKeyboard.Notifications.ObserveWillShow((sender, args) =>
//            {
//                isKeyboardStateChanging = true;
//                var keyboardBounds = UIKeyboard.BoundsFromNotification(args.Notification);
//                _ = ((HubWebView)Element).HandleKeyboardStateChange(true, keyboardBounds.Size.Height);
//            });

//            UIKeyboard.Notifications.ObserveDidShow((sender, args) =>
//            {
//                isKeyboardStateChanging = false;
//            });

//            // Handle keyboard hide notifications
//            UIKeyboard.Notifications.ObserveWillHide((sender, args) =>
//            {
//                isKeyboardStateChanging = true;
//                _ = ((HubWebView)Element).HandleKeyboardStateChange(false, 0);
//            });

//            UIKeyboard.Notifications.ObserveDidHide((sender, args) =>
//            {
//                isKeyboardStateChanging = false;
//            });
//        }

//        protected override void OnElementChanged(VisualElementChangedEventArgs e)
//        {
//            base.OnElementChanged(e);

//            if (e.OldElement == null)
//            {
//                // Override the WKNavigationDelegate for the WKWebView
//                NavigationDelegate = new WebNavigationDelegate(NavigationDelegate, this);
//                ScrollView.ScrollEnabled = false;
//                ScrollView.Delegate = new ScrollDelegate(this);

//                if (ScrollView.PinchGestureRecognizer != null)
//                {
//                    ScrollView.PinchGestureRecognizer.Enabled = false;
//                }

//                var bottomPadding = ScreenInsets.Bottom == 0 ? 40 : ScreenInsets.Bottom * 2;
//                ((HubWebView)e.NewElement).Margin = new Thickness
//                {
//                    Bottom = bottomPadding + ScreenInsets.Top
//                };
//                e.NewElement.Focus();
//            }
//        }

//        public HubWebView(WKWebViewConfiguration config) : base(config)
//        {
//            userController = config.UserContentController;
//            userController.AddScriptMessageHandler(this, "rowndIosSDK");
//            userController.AddScriptMessageHandler(new JsLoggingToConsole(), "rowndDeviceLogger");
//        }

//        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
//        {
//            ((HubWebView)Element).HandleHubMessage(message.Body.ToString());
//        }

//        private class ScrollDelegate : UIScrollViewDelegate
//        {
//            private readonly HubWebViewRenderer _renderer;

//            public ScrollDelegate(HubWebViewRenderer renderer)
//            {
//                _renderer = renderer;
//            }

//            public override void Scrolled(UIScrollView scrollView)
//            {
//                if (!_renderer.isKeyboardStateChanging)
//                {
//                    return;
//                }

//                if (scrollView.ContentOffset.Y != 0)
//                {
//                    scrollView.ContentOffset = new CoreGraphics.CGPoint
//                    {
//                        Y = 0,
//                        X = scrollView.ContentOffset.X
//                    };
//                }
//            }
//        }

//        #region WKNavigationDelegate
//        private class WebNavigationDelegate : WKNavigationDelegate
//        {
//            private HubWebViewRenderer _renderer;

//            private readonly IWKNavigationDelegate _defaultDelegate;

//            public WebNavigationDelegate(IWKNavigationDelegate defaultDelegate, HubWebViewRenderer renderer)
//            {
//                if (defaultDelegate == null)
//                {
//                    throw new ArgumentNullException(nameof(defaultDelegate));
//                }

//                _defaultDelegate = defaultDelegate;
//                _renderer = renderer;
//            }

//            // Be sure to implement ALL methods with are implemented by the CustomWebViewNavigationDelete, which is a private class, so we can't override it simply
//            // https://github.com/xamarin/Xamarin.Forms/blob/4.6.0/Xamarin.Forms.Platform.iOS/Renderers/WkWebViewRenderer.cs
//            public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
//            {
//                _defaultDelegate.DidFailNavigation(webView, navigation, error);
//            }

//            public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
//            {
//                _defaultDelegate.DidFinishNavigation(webView, navigation);
//            }

//            public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
//            {
//                _defaultDelegate.DidStartProvisionalNavigation(webView, navigation);
//            }

//            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
//            {
//                if (navigationAction.NavigationType == WKNavigationType.LinkActivated)
//                {
//                    var url = navigationAction.Request.Url;
//                    if (((HubWebView)_renderer.Element).HandleLinkActivation(url.ToString()))
//                    {
//                        decisionHandler(WKNavigationActionPolicy.Allow);
//                    }
//                    else
//                    {
//                        decisionHandler(WKNavigationActionPolicy.Cancel);
//                    }

//                    return;
//                }

//                decisionHandler(WKNavigationActionPolicy.Allow);
//            }
//        }
//        #endregion
//    }

//    public class JsLoggingToConsole : WkWebViewRenderer, IWKScriptMessageHandler
//    {
//        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
//        {
//            Console.WriteLine(message.Body.ToString());
//        }
//    }
//}
