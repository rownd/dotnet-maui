using System;
using Android.Content;
using AndroidX.Core.View;
using Microsoft.Extensions.Logging;
using Rownd.Maui.Android.Hub;
using Rownd.Maui.Core;
using Rownd.Maui.Utils;
using AWebView = Android.Webkit.WebView;

namespace Rownd.Maui.Hub
{
    partial class HubWebView
    {
        private Context _context;

        private AWebView PlatformWebView => (AWebView)Handler!.PlatformView!;

        private static int PixelsToDp(float pixelValue)
        {
            var density = DeviceDisplay.MainDisplayInfo.Density;
            var dp = (int)(pixelValue / density);
            return dp;
        }

        private partial async Task InitializeWebView()
        {
            PlatformWebView.Settings.JavaScriptEnabled = true;
            PlatformWebView.Settings.SetSupportMultipleWindows(false);
            PlatformWebView.AddJavascriptInterface(new JSBridge(this), "rowndAndroidSDK");
            PlatformWebView.Settings.UserAgentString = Constants.DEFAULT_WEB_USER_AGENT;

            // Listen for layout changes like the soft keyboard opening
            var activity = await Platform.WaitForActivityAsync();
            var rootView = activity.Window?.DecorView.RootView;
            rootView?.ViewTreeObserver?.AddOnGlobalLayoutListener(new CustomLayoutListener(this));

            if (rootView == null)
            {
                Loggers.Default.LogWarning("Failed to get root view from activity. This may cause rendering issues.");
                return;
            }

            // Set bottom margin to prevent system nav overlap
            var navInsets = ViewCompat.GetRootWindowInsets(rootView)?.GetInsets(WindowInsetsCompat.Type.SystemBars());

            Margin = new Thickness
            {
                Bottom = PixelsToDp(navInsets.Bottom)
            };
        }

        private partial void NavigateCore(string url)
        {
            PlatformWebView.LoadUrl(url);
        }
    }
}
