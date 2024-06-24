using CoreGraphics;
using UIKit;
using WebKit;

namespace Rownd.Maui.Hub
{
    internal class HubWKWebView : WKWebView
    {
        public HubWKWebView(CGRect frame, WKWebViewConfiguration configuration) : base(frame, configuration) { }

        public override UIView? InputAccessoryView => null;
    }
}
