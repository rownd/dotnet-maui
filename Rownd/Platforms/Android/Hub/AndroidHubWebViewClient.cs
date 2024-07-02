using Microsoft.Maui.Platform;

namespace Rownd.Maui.Hub
{
    internal class AndroidHubWebViewClient : MauiWebViewClient
    {
        private readonly HubWebViewHandler _handler;

        internal AndroidHubWebViewClient(HubWebViewHandler handler) : base(handler)
        {
            _handler = handler;
        }
    }
}
