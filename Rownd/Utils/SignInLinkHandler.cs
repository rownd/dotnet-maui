using Microsoft.Extensions.Logging;
using RestSharp;
using Rownd.Maui.Core;
using Rownd.Maui.Models;
using Rownd.Maui.Models.Domain;
using Rownd.Maui.Models.Repos;

namespace Rownd.Maui.Utils
{
    internal partial class SignInLinkHandler
    {
        private partial Task<string?> HandleSignInLink();

#if !ANDROID && !IOS && !MACCATALYST && !WINDOWS
        private partial Task<string?> HandleSignInLink() => throw null!;
#endif

        public static SignInLinkHandler Get()
        {
            return Shared.ServiceProvider.GetRequiredService<SignInLinkHandler>();
        }

        public async void HandleSignInLinkIfPresent()
        {
            var signInLink = await GetSignInLinkIfPresent();

            if (signInLink == null)
            {
                return;
            }

            await AuthenticateWithSignInLink(signInLink);
        }

        private async Task<string?> GetSignInLinkIfPresent()
        {
            return await HandleSignInLink();
        }

        private async Task AuthenticateWithSignInLink(string url)
        {
            try
            {
                var urlObj = new Uri(url);
                if (urlObj.Fragment != null)
                {
                    url = url.Replace($"#{urlObj.Fragment}", "");
                }

                var apiClient = ApiClient.Get();
                var request = new RestRequest(url);
                var response = await apiClient.Client.ExecuteGetAsync<AuthState>(request);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return;
                }

                if (response?.Data?.AccessToken == null)
                {
                    return;
                }

                var stateRepo = StateRepo.Get();
                MainThread.BeginInvokeOnMainThread(() =>
                    stateRepo.Store.Dispatch(new StateActions.SetAuthState()
                    {
                        AuthState = new AuthState
                        {
                            AccessToken = response.Data.AccessToken,
                            RefreshToken = response.Data.RefreshToken,
                        },
                    })
                );
            }
            catch (Exception ex)
            {
                Loggers.Default.LogError("Failed handling sign-in link '{url}'. Exception: {ex}", url, ex);
            }
        }
    }
}
