using Android.App;
using Android.Content.PM;
using Android.OS;
using RowndMauiExample;

namespace RowndMauiExampleNew.Android;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        //SetContentView(Resource.Layout.activity_main);
        Platform.Init(this, savedInstanceState);
    }
}
