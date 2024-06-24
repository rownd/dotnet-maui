//using Android.Content;
//using Android.Util;
//using AndroidX.Core.View;
//using Rownd.Maui.Android.Hub;
//using Rownd.Maui.Core;
//using Rownd.Maui.Hub;
//using Xamarin.Essentials;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;
//using static Android.Content.Res.Resources;
//using XEssentials = Xamarin.Essentials;

//[assembly: ExportRenderer(typeof(HubWebView), typeof(HubWebViewRenderer))]
//namespace Rownd.Maui.Android.Hub
//{
//    public class HubWebViewRenderer : WebViewRenderer
//    {
//        private Context _context;

//        private static int PixelsToDp(float pixelValue)
//        {
//            var density = DeviceDisplay.MainDisplayInfo.Density;
//            var dp = (int)(pixelValue / density);
//            return dp;
//        }

//        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
//        {
//            base.OnElementChanged(e);

//            if (e.OldElement != null)
//            {
//                Control.RemoveJavascriptInterface("rowndAndroidSDK");
//            }

//            if (e.NewElement != null)
//            {
//                Control.Settings.SetSupportMultipleWindows(false);
//                Control.AddJavascriptInterface(new JSBridge(this), "rowndAndroidSDK");
//                Control.Settings.UserAgentString = Constants.DEFAULT_WEB_USER_AGENT;

//                // Listen for layout changes like the soft keyboard opening
//                var rootView = XEssentials.Platform.CurrentActivity.Window.DecorView.RootView;
//                rootView.ViewTreeObserver.AddOnGlobalLayoutListener(new CustomLayoutListener(this));

//                // Set bottom margin to prevent system nav overlap
//                var navInsets = ViewCompat.GetRootWindowInsets(rootView)?.GetInsets(WindowInsetsCompat.Type.SystemBars());

//                e.NewElement.Margin = new Thickness
//                {
//                    Bottom = PixelsToDp(navInsets.Bottom)
//                };
//            }
//        }

//        public HubWebViewRenderer(Context context) : base(context)
//        {
//            _context = context;
//        }
//    }
//}