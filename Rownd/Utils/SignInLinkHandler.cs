using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Rownd.Maui.Core;
using Rownd.Maui.Models;
using Rownd.Maui.Models.Domain;
using Rownd.Maui.Models.Repos;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Rownd.Maui.Utils
{
    public class SignInLinkHandler
    {
        public SignInLinkHandler()
        {
        }

        public static SignInLinkHandler Get()
        {
            return Shared.ServiceProvider.GetService<SignInLinkHandler>();
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

        private async Task<string> GetSignInLinkIfPresent()
        {
            var signInSvc = DependencyService.Get<ISignInLinkHandler>();
            return await signInSvc.HandleSignInLink();
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
                Device.BeginInvokeOnMainThread(() =>
                    stateRepo.Store.Dispatch(new StateActions.SetAuthState()
                    {
                        AuthState = new AuthState
                        {
                            AccessToken = response.Data.AccessToken,
                            RefreshToken = response.Data.RefreshToken
                        }
                    })
);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed handling sign-in link '{url}'. Exception: {ex}");
            }
        }
    }
}
