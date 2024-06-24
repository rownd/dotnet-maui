using Microsoft.Maui.Platform;
using System;

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
