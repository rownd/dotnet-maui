using Microsoft.Maui.Handlers;

namespace Rownd.Maui.Hub
{
    internal partial class HubWebViewHandler : WebViewHandler
    {
        public static IPropertyMapper<IWebView, IWebViewHandler> HubWebViewHandlerMapper = new PropertyMapper<IWebView, IWebViewHandler>(WebViewHandler.Mapper)
        {
#if __ANDROID__
            [nameof(global::Android.Webkit.WebViewClient)] = MapHubWebViewClient,
#endif
        };

        public HubWebViewHandler() : base(HubWebViewHandlerMapper, CommandMapper)
        {
        }

        public HubWebViewHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null)
            : base(mapper ?? HubWebViewHandlerMapper, commandMapper ?? CommandMapper)
        {
        }

#if ANDROID
        public static void MapHubWebViewClient(IWebViewHandler handler, IWebView webView)
        {
            if (handler is HubWebViewHandler platformHandler)
            {
                var webViewClient = new AndroidHubWebViewClient(platformHandler);
                handler.PlatformView.SetWebViewClient(webViewClient);

                // TODO: There doesn't seem to be a way to override MapWebViewClient() in maui/src/Core/src/Handlers/WebView/WebViewHandler.Android.cs
                // in such a way that it knows of the custom MauiWebViewClient that we're creating. So, we use private reflection to set it on the
                // instance. We might end up duplicating WebView/BlazorWebView anyway, in which case we wouldn't need this workaround.
                var webViewClientField = typeof(WebViewHandler).GetField("_webViewClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy);

                // Starting in .NET 8.0 the private field is gone and this call isn't necessary, so we only set if it needed
                webViewClientField?.SetValue(handler, webViewClient);
            }
        }
#endif
    }
}