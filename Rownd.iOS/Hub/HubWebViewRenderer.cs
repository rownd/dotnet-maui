﻿using System;
using System.ComponentModel.Design;
using Foundation;
using Rownd.Xamarin.Core;
using Rownd.Xamarin.Hub;
using Rownd.Xamarin.iOS.Hub;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HubWebView), typeof(HubWebViewRenderer))]
namespace Rownd.Xamarin.iOS.Hub
{
    public class HubWebViewRenderer : WkWebViewRenderer, IWKScriptMessageHandler
    {
        private WKUserContentController userController;
        private NSObject notification;

        public HubWebViewRenderer() : this(new WKWebViewConfiguration())
        {
            CustomUserAgent = Constants.DEFAULT_WEB_USER_AGENT;
            notification = UIKeyboard.Notifications.ObserveDidShow((sender, args) =>
            {
                ((HubWebView)Element).HandleKeyboardStateChange(true);
            });
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                // Override the WKNavigationDelegate for the WKWebView
                NavigationDelegate = new WebNavigationDelegate(NavigationDelegate, this);
            }
        }

        public HubWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
            userController = config.UserContentController;
            userController.AddScriptMessageHandler(this, "rowndIosSDK");
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            ((HubWebView)Element).HandleHubMessage(message.Body.ToString());
        }

        #region WKNavigationDelegate
        private class WebNavigationDelegate : WKNavigationDelegate
        {
            private HubWebViewRenderer _renderer;

            private readonly IWKNavigationDelegate _defaultDelegate;

            public WebNavigationDelegate(IWKNavigationDelegate defaultDelegate, HubWebViewRenderer renderer)
            {
                if (defaultDelegate == null)
                {
                    throw new ArgumentNullException(nameof(defaultDelegate));
                }

                _defaultDelegate = defaultDelegate;
                _renderer = renderer;
            }

            // Be sure to implement ALL methods with are implemented by the CustomWebViewNavigationDelete, which is a private class, so we can't override it simply
            // https://github.com/xamarin/Xamarin.Forms/blob/4.6.0/Xamarin.Forms.Platform.iOS/Renderers/WkWebViewRenderer.cs
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

            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                if (navigationAction.NavigationType == WKNavigationType.LinkActivated)
                {
                    var url = navigationAction.Request.Url;
                    if (((HubWebView)_renderer.Element).HandleLinkActivation(url.ToString()))
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
    }
}
