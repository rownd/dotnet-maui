using Rownd.Maui.Hub;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HubWebViewServiceCollectionExtensions
    {
        public static void AddHybridWebView(this IServiceCollection services)
        {
            services.ConfigureMauiHandlers(static handlers => handlers.AddHandler<HubWebView, HubWebViewHandler>());
        }
    }
}
