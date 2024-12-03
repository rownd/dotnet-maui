using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Rownd.Maui.Utils;

namespace RowndMauiExample;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
    {
        builder.Services.AddLogging(
            configure =>
            {
                configure.SetMinimumLevel(LogLevel.Trace);
                configure.AddFilter("rownd", LogLevel.Trace);
                configure.AddDebug();
                configure.AddConsole();
            }
        );

        builder
            .UseMauiCompatibility()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseRownd();

        // TODO: Add the entry points to your Apps here.
        // See also: https://learn.microsoft.com/dotnet/maui/fundamentals/app-lifecycle
        builder.Services.AddTransient<AppShell>();

        return builder;
    }
}
