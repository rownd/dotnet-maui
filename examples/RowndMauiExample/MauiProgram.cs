using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace RowndMauiExample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseSharedMauiApp();
                //.ConfigureMauiHandlers(handlers =>
                //{
                //    // Register ALL handlers in the Xamarin Community Toolkit assembly
                //    handlers.AddCompatibilityRenderers(typeof(Xamarin.CommunityToolkit.UI.Views.MediaElementRenderer).Assembly);
                //});
            return builder.Build();
        }
    }
}