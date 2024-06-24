using System;
using System.Threading.Tasks;
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
    }
}

