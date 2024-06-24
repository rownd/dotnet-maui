using System;
using Android.Webkit;
using Java.Interop;
using Rownd.Maui.Hub;

namespace Rownd.Maui.Android.Hub
{
    internal class JSBridge : Java.Lang.Object
    {
        private readonly WeakReference<HubWebView> hubWebView;

        internal JSBridge(HubWebView hubWebView)
        {
            this.hubWebView = new WeakReference<HubWebView>(hubWebView);
        }

        [JavascriptInterface]
        [Export("postMessage")]
        public void InvokeAction(string message)
        {
            if (this.hubWebView != null && this.hubWebView.TryGetTarget(out var hubWebView))
            {
                hubWebView.HandleHubMessage(message);
            }
        }
    }
}
