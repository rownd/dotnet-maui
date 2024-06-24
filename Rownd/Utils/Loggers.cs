using Microsoft.Extensions.Logging;

namespace Rownd.Maui.Utils
{
    internal class Loggers
    {
        internal static readonly Loggers Shared = new();

        internal ILogger Default { get; }
        internal ILogger HubWebView { get; }

        private Loggers()
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            Default = factory.CreateLogger("rownd");
            HubWebView = factory.CreateLogger("rownd.hub");
        }
    }
}
