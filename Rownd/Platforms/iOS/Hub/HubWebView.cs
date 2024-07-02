using System;
using System.Threading.Tasks;
using Foundation;
using WebKit;

namespace Rownd.Maui.Hub
{
    partial class HubWebView
    {
        private WKWebView PlatformWebView => (WKWebView)Handler!.PlatformView!;

        private partial Task InitializeWebView()
        {
            return Task.CompletedTask;
        }

        private partial void NavigateCore(string url)
        {
            using var nsUrl = new NSUrl(url);
            using var request = new NSUrlRequest(nsUrl);

            PlatformWebView.LoadRequest(request);
        }
    }
}
