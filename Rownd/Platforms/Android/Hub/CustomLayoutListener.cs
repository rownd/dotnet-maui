using Android.OS;
using Android.Util;
using Android.Views;
using AndroidX.Core.View;
using Rownd.Maui.Hub;

namespace Rownd.Maui.Android.Hub
{
    internal class CustomLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private readonly HubWebView hubWebView;
        private bool isKeyboardOpen = false;

        internal CustomLayoutListener(HubWebView hubWebView) => this.hubWebView = hubWebView;

        public void OnGlobalLayout()
        {
            var window = Platform.CurrentActivity?.Window;
            var rootView = window?.DecorView.RootView;

            var r = new global::Android.Graphics.Rect();
            window?.DecorView.GetWindowVisibleDisplayFrame(r);

            var navInsets = ViewCompat.GetRootWindowInsets(rootView)?.GetInsets(WindowInsetsCompat.Type.NavigationBars());

            var isKeyboardOpen = ViewCompat.GetRootWindowInsets(rootView)?.IsVisible(WindowInsetsCompat.Type.Ime()) ?? true;
            var imeInsets = ViewCompat.GetRootWindowInsets(rootView)?.GetInsets(WindowInsetsCompat.Type.Ime());

            if (isKeyboardOpen != this.isKeyboardOpen)
            {
                var keyboardHeightDip = Build.VERSION.SdkInt > BuildVersionCodes.Q
                    ? ((imeInsets?.Bottom - navInsets?.Bottom) / DeviceDisplay.Current.MainDisplayInfo.Density) ?? 0
                    : 0;
                _ = hubWebView.HandleKeyboardStateChange(isKeyboardOpen, keyboardHeightDip);
                this.isKeyboardOpen = isKeyboardOpen;
            }
        }
    }
}