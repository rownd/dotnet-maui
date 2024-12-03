using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Rownd.Maui.Core;
using Rownd.Maui.Models.Domain;
using Rownd.Maui.Utils;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Rownd.Maui.Models.Repos
{
    public class AuthRepo
    {
        public class RefreshTokenExpiredException : Exception
        {
            public RefreshTokenExpiredException() { }

            public RefreshTokenExpiredException(string message) : base(message) { }
        }

        private readonly StateRepo stateRepo = StateRepo.Get();

        private readonly object refreshLock = new();
        private Task refreshTask = Task.CompletedTask;

        private class TokenRequestBody
        {
            [JsonProperty("app_id")]
            public string AppId { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("id_token")]
            public string IdToken { get; set; }
        }

        public static AuthRepo Get()
        {
            return Shared.ServiceProvider.GetService<AuthRepo>();
        }

        public AuthRepo()
        {
        }

        public async Task<string> GetAccessToken()
        {
            var authState = stateRepo.Store.State.Auth;

            if (authState?.AccessToken == null)
            {
                throw new RowndException("No access token available. Request a sign in first.");
            }

            if (authState?.IsAccessTokenValid == false)
            {
                authState = await RefreshToken();
            }

            return authState?.AccessToken;
        }

        public async Task<AuthState> GetAccessToken(string token)
        {
            var apiClient = ApiClient.Get();
            try
            {
                var request = new RestRequest("hub/auth/token")
                    .AddBody(new TokenRequestBody
                    {
                        AppId = stateRepo.Store.State.AppConfig.Id,
                        IdToken = token
                    });
                var response = await apiClient.Client.ExecutePostAsync<AuthState>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // This is the only case where the user should be signed out automatically, b/c the app
                    // won't be able to continue.
                    Shared.Rownd.SignOut();
                }

                MainThread.BeginInvokeOnMainThread(() =>
                    stateRepo.Store.Dispatch(new StateActions.SetAuthState()
                    {
                        AuthState = response.Data
                    })
                );

                return response.Data;
            }
            catch (Exception ex)
            {
                Loggers.Default.LogCritical("Failed to exchange or refresh access token: {ex}", ex);
                return null;
            }
        }

        public async Task AuthenticateUsingSignInLink(string link)
        {
            var apiClient = ApiClient.Get();
            var request = new RestRequest(link);
            var response = await apiClient.Client.ExecuteGetAsync(request);

            // TODO: Handle response payload
        }

        internal Task<AuthState> RefreshToken()
        {
            lock (refreshLock)
            {
                if (!refreshTask.IsCompleted)
                {
                    return refreshTask as Task<AuthState>;
                }

                refreshTask = Task.Run(async () =>
                {
                    try
                    {
                        var result = await MakeRefreshTokenRequest();

                        // Replace in-memory tokens for immediate use, but still dispatch
                        stateRepo.Store.State.Auth.AccessToken = result.AccessToken;
                        stateRepo.Store.State.Auth.RefreshToken = result.RefreshToken;

                        MainThread.BeginInvokeOnMainThread(() =>
                            stateRepo.Store.Dispatch(new StateActions.SetAuthState()
                            {
                                AuthState = result
                            })
                        );

                        return result;
                    }
                    catch (RefreshTokenExpiredException ex)
                    {
                        Loggers.Default.LogWarning("Failed to refresh token. It was likely expired. User will be signed out. Reason: {ex}", ex);
                        MainThread.BeginInvokeOnMainThread(() => Shared.Rownd.SignOut());
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Loggers.Default.LogWarning("Failed to refresh token. This may be recoverable. Reason: {ex}", ex);
                        throw new RowndException("Failed to refresh token. This may be recoverable.", ex.InnerException);
                    }
                    finally
                    {
                        refreshTask = Task.CompletedTask;
                    }
                });

                return refreshTask as Task<AuthState>;
            }
        }

        private async Task<AuthState> MakeRefreshTokenRequest()
        {
            var request = new RestRequest("hub/auth/token")
                .AddBody(new TokenRequestBody
                {
                    AppId = stateRepo.Store.State.AppConfig.Id,
                    RefreshToken = stateRepo.Store.State.Auth.RefreshToken
                });
            var apiClient = ApiClient.Get();
            var result = await apiClient.Client.ExecutePostAsync<AuthState>(request);

            if (result.ResponseStatus == ResponseStatus.Completed)
            {
                return result.Data;
            }
            else if (result.ResponseStatus == ResponseStatus.Error && result.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new RefreshTokenExpiredException(result.Content);
            }

            throw new RowndException(result.Content);
        }

        public async Task HandleThirdPartySignIn(ThirdPartySignInData data)
        {
            var tokenResp = await GetAccessToken(data.Token);

            Shared.Rownd.RequestSignIn(new RowndSignInJsOptions
            {
                SignInStep = SignInStep.Success,
                Intent = data.Intent,
                UserType = tokenResp.UserType,
            });

            // Prevents too-rapid UI state changes
            await Task.Delay(TimeSpan.FromSeconds(2));

            stateRepo.Store.Dispatch(new StateActions.SetAuthState
            {
                AuthState = new AuthState
                {
                    AccessToken = tokenResp.AccessToken,
                    RefreshToken = tokenResp.RefreshToken,
                    UserType = tokenResp.UserType
                }
            });

            // TODO: Set last sign-in method

            var userRepo = UserRepo.GetInstance();
            await userRepo.FetchUser();
            var userData = userRepo.Get();

            foreach (var field in data.UserData)
            {
                if (string.IsNullOrEmpty(field.Value))
                {
                    continue;
                }

                userData[field.Key] = field.Value;
            }

            userRepo.Set(userData);
        }
    }
}
