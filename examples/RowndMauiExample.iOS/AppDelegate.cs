using Foundation;
using RowndMauiExample;
using UIKit;

namespace RowndMauiExampleNew.iOS;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    //public override UIWindow? Window
    //{
    //    get;
    //    set;
    //}

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    //public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    //{
    //    //Rownd.Maui.iOS.Boot.Init();

    //    // create a new window instance based on the screen size
    //    //Window = new UIWindow(UIScreen.MainScreen.Bounds);

    //    //// create a UIViewController with a single UILabel
    //    //var vc = new UIViewController();
    //    //vc.View!.AddSubview(new UILabel(Window!.Frame)
    //    //{
    //    //    BackgroundColor = UIColor.SystemBackground,
    //    //    TextAlignment = UITextAlignment.Center,
    //    //    Text = "Hello, iOS!",
    //    //    AutoresizingMask = UIViewAutoresizing.All,
    //    //});
    //    //Window.RootViewController = vc;

    //    //// make the window visible
    //    //Window.MakeKeyAndVisible();

    //    return base.FinishedLaunching(application, launchOptions);
    //}
}
