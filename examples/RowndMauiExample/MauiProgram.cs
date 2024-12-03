using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace RowndMauiExample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseSharedMauiApp();

            var app = builder.Build();

            var loggerFactory = app.Services.GetService<ILoggerFactory>();
            Rownd.Maui.Utils.Loggers.SetLogFactory(loggerFactory);

            return app;
        }
    }
}