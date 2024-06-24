//using Android.Views;

//namespace Rownd.Maui.Controls
//{
//    public static partial class PageExtensions
//    {
//        public static partial int GetWindowSoftInputModeAdjust()
//        {
//            var window = Platform.CurrentActivity?.Window;
//            return (int)(window?.Attributes?.SoftInputMode ?? 0 & SoftInput.MaskAdjust);
//        }

//        public static partial void SetWindowSoftInputModeAdjust(int adjustMode)
//        {
//            var window = Platform.CurrentActivity?.Window;
//            if (window != null)
//            {
//                window.SetSoftInputMode((SoftInput)adjustMode);
//            }
//        }
//        //private static readonly IDictionary<Type, WindowSoftInputModeAdjust> OriginalWindowSoftInputModeAdjusts = new Dictionary<Type, WindowSoftInputModeAdjust>();

//        //public static void UseWindowSoftInputModeAdjust(this Page page, WindowSoftInputModeAdjust windowSoftInputModeAdjust)
//        //{
//        //    //var activity = MauiApplication.Current.ApplicationContext as Activity;
//        //    var activity = Platform.CurrentActivity;
//        //    var window = activity?.Window;

//        //    var pageType = page.GetType();
//        //    if (!OriginalWindowSoftInputModeAdjusts.ContainsKey(pageType))
//        //    {
//        //        WindowSoftInputModeAdjust originalWindowSoftInputModeAdjust = (WindowSoftInputModeAdjust)(window?.Attributes?.SoftInputMode ?? 0 & SoftInput.MaskAdjust);
//        //        OriginalWindowSoftInputModeAdjusts.Add(pageType, originalWindowSoftInputModeAdjust);
//        //    }

//        //    activity.UseWindow(windowSoftInputModeAdjust);
//        //}

//        //public static void ResetWindowSoftInputModeAdjust(this Page page)
//        //{
//        //    var pageType = page.GetType();

//        //    if (OriginalWindowSoftInputModeAdjusts.TryGetValue(pageType, out var originalWindowSoftInputModeAdjust))
//        //    {
//        //        OriginalWindowSoftInputModeAdjusts.Remove(pageType);

//        //        var platformElementConfiguration = Microsoft.Maui.Controls.Application.Current;
//        //        platformElementConfiguration?.UseWindowSoftInputModeAdjust(originalWindowSoftInputModeAdjust);
//        //    }
//        //}

//        //public static int GetWindowSoftInputModeAdjust()
//        //{
//        //    var window = Platform.CurrentActivity?.Window;
//        //    return window?.Attributes.SoftInputMode & SoftInput.AdjustMask ?? -1;
//        //}

//        //public void UseWindowSoftInputModeAdjust(int adjustMode)
//        //{
//        //    var window = Platform.CurrentActivity?.Window;
//        //    if (window != null)
//        //    {
//        //        window.SetSoftInputMode(adjustMode);
//        //    }
//        //}
//    }
//}
