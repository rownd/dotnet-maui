using HttpTracer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rownd.Maui.Utils
{
    public static class Loggers
    {
        private static IDictionary<string, ILogger> _loggers = new Dictionary<string, ILogger>();

        private static ILoggerFactory _loggerFactory = new LoggerFactory();

        internal static ILogger Default
        {
            get
            {
                return GetLogger("rownd");
            }
        }

        internal static ILogger HubWebView
        {
            get
            {
                return GetLogger("rownd.hub");
            }
        }

        private static HttpLogger _httpLogger;

        internal static HttpLogger Http
        {
            get
            {
                if (_httpLogger == null)
                {
                    _httpLogger = new HttpLogger(GetLogger("rownd.http"));
                }

                return _httpLogger;
            }
        }

        public static void SetLogFactory(ILoggerFactory factory)
        {
            _loggerFactory?.Dispose();
            _loggerFactory = factory;
            _loggers.Clear();
        }

        public static ILogger GetLogger(string category)
        {
            if (!_loggers.ContainsKey(category))
            {
                _loggers[category] = _loggerFactory?.CreateLogger(category) ?? NullLogger.Instance;
            }
            return _loggers[category];
        }
    }

    internal class HttpLogger : HttpTracer.Logger.ILogger
    {
        private readonly ILogger _logger;

        internal HttpLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(string message)
        {
            _logger.LogTrace(message);
        }
    }
}
