using CoreGraphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;
using WebKit;

namespace Rownd.Maui.Hub
{
    internal class HubWKWebView : MauiWKWebView
    {
        public HubWKWebView(CGRect frame, WebViewHandler handler, WKWebViewConfiguration configuration) : base(frame, handler, configuration) { }

        public override UIView InputAccessoryView => null!;
    }
}
