using System;
using Rownd.Maui.Hub;

namespace Rownd.Maui.Utils
{
    public static class MauiConfig
    {
        public static MauiAppBuilder UseRownd(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(HubWebView), typeof(HubWebViewHandler));
            });

            return builder;
        }
    }
}
