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
            var rootView = Platform.CurrentActivity.Window.DecorView.RootView;

            var r = new global::Android.Graphics.Rect();
            Platform.CurrentActivity.Window.DecorView.GetWindowVisibleDisplayFrame(r);

            var navInsets = ViewCompat.GetRootWindowInsets(rootView)?.GetInsets(WindowInsetsCompat.Type.NavigationBars());

            var isKeyboardOpen = ViewCompat.GetRootWindowInsets(rootView)?.IsVisible(WindowInsetsCompat.Type.Ime()) ?? true;
            if (isKeyboardOpen != this.isKeyboardOpen)
            {
                hubWebView.HandleKeyboardStateChange(isKeyboardOpen, rootView.Height - r.Bottom - (navInsets?.Bottom ?? 0));
                this.isKeyboardOpen = isKeyboardOpen;
            }
        }
    }
}