using AndroidX.Core.View;
using Rownd.Maui.Utils;

namespace Rownd.Maui.Android
{
    internal static class PlatformUtils
    {
        internal static Thickness GetWindowSafeArea()
        {
            var rootView = Platform.CurrentActivity?.Window?.DecorView.RootView;
            var insets = ViewCompat.GetRootWindowInsets(rootView)?.GetInsets(WindowInsetsCompat.Type.SystemBars());

            return new Thickness
            {
                Top = PixelsToDp(insets.Top),
                Bottom = PixelsToDp(insets.Bottom),
                Right = PixelsToDp(insets.Right),
                Left = PixelsToDp(insets.Left),
            };
        }

        internal static double PixelsToDp(float pixelValue)
        {
            var density = DeviceDisplay.MainDisplayInfo.Density;
            var dp = pixelValue / density;
            return dp;
        }

        internal static double GetWindowHeight()
        {
            var rootView = Platform.CurrentActivity?.Window?.DecorView.RootView;
            return PixelsToDp(rootView?.Height ?? 0);
        }
    }
}
