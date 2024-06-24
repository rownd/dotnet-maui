using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace Rownd.Maui.Controls
{
    public static class PageExtensions
    {
        private static readonly IDictionary<Type, WindowSoftInputModeAdjust> OriginalWindowSoftInputModeAdjusts = new Dictionary<Type, WindowSoftInputModeAdjust>();

        public static void UseWindowSoftInputModeAdjust(this Page page, WindowSoftInputModeAdjust windowSoftInputModeAdjust)
        {
            var platformElementConfiguration = Microsoft.Maui.Controls.Application.Current?.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>();

            var pageType = page.GetType();
            if (!OriginalWindowSoftInputModeAdjusts.ContainsKey(pageType))
            {
                var originalWindowSoftInputModeAdjust = platformElementConfiguration.GetWindowSoftInputModeAdjust();
                OriginalWindowSoftInputModeAdjusts.Add(pageType, originalWindowSoftInputModeAdjust);
            }

            platformElementConfiguration.UseWindowSoftInputModeAdjust(windowSoftInputModeAdjust);
        }

        public static void ResetWindowSoftInputModeAdjust(this Page page)
        {
            var pageType = page.GetType();

            if (OriginalWindowSoftInputModeAdjusts.TryGetValue(pageType, out var originalWindowSoftInputModeAdjust))
            {
                OriginalWindowSoftInputModeAdjusts.Remove(pageType);

                var platformElementConfiguration = Microsoft.Maui.Controls.Application.Current?.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>();
                platformElementConfiguration.UseWindowSoftInputModeAdjust(originalWindowSoftInputModeAdjust);
            }
        }
    }
    //public static partial class PageExtensions
    //{
    //    private static readonly IDictionary<Type, int> OriginalWindowSoftInputModeAdjusts = new Dictionary<Type, int>();

    //    public static void UseWindowSoftInputModeAdjust(this Page page, WindowSoftInputModeAdjust windowSoftInputModeAdjust)
    //    {
    //        var pageType = page.GetType();
    //        if (!OriginalWindowSoftInputModeAdjusts.ContainsKey(pageType))
    //        {
    //            var originalWindowSoftInputModeAdjust = GetWindowSoftInputModeAdjust();
    //            OriginalWindowSoftInputModeAdjusts.Add(pageType, originalWindowSoftInputModeAdjust);
    //        }

    //        SetWindowSoftInputModeAdjust(windowSoftInputModeAdjust);
    //        Microsoft.Maui.Controls.Application.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

    //    }

    //    public static void ResetWindowSoftInputModeAdjust(this Page page)
    //    {
    //        var pageType = page.GetType();

    //        if (OriginalWindowSoftInputModeAdjusts.TryGetValue(pageType, out var originalWindowSoftInputModeAdjust))
    //        {
    //            OriginalWindowSoftInputModeAdjusts.Remove(pageType);
    //            SetWindowSoftInputModeAdjust(originalWindowSoftInputModeAdjust);
    //        }
    //    }

    //    // Platform-specific methods to be implemented in Android project
    //    public static partial int GetWindowSoftInputModeAdjust();
    //    public static partial void SetWindowSoftInputModeAdjust(WindowSoftInputModeAdjust adjustMode);
    //}
}
