using Microsoft.Extensions.Logging;
using RestSharp;
using Rownd.Maui.Core;
using Rownd.Maui.Models.Domain;
using Rownd.Maui.Utils;

namespace Rownd.Maui.Models.Repos
{
    public class AppConfigRepo
    {
        private readonly StateRepo stateRepo = StateRepo.Get();

        private class AppConfigResponse
        {
            public AppState? App;
        }

        public static AppConfigRepo Get()
        {
            return Shared.ServiceProvider.GetRequiredService<AppConfigRepo>();
        }

        public async Task<AppState?> LoadAppConfigAsync()
        {
            try
            {
                var apiClient = ApiClient.Get();
                var response = await apiClient.Client.GetAsync<AppConfigResponse>("hub/app-config");

                if (response?.App == null)
                {
                    throw new RowndException($"Missing app config in response");
                }

                MainThread.BeginInvokeOnMainThread(() =>
                    stateRepo.Store.Dispatch(new StateActions.SetAppConfig()
                    {
                        AppConfig = response.App
                    })
                );
                return response.App;
            }
            catch (Exception ex)
            {
                Loggers.Shared.Default.LogError(ex, "Failed to fetch app config");
                return null;
            }
        }
    }
}
