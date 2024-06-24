using System;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Rownd.Maui.Models.Repos;

namespace Rownd.Maui.Core
{
    public class ApiAuthenticator : IAuthenticator
    {
        public async ValueTask Authenticate(IRestClient client, RestRequest request)
        {
            var stateRepo = StateRepo.Get();

            if (stateRepo?.Store?.State?.Auth?.IsAuthenticated == true && !(request.Resource.Contains("/token") || request.Resource.Contains("/app-config")))
            {
                var accessToken = await AuthRepo.Get().GetAccessToken();
                request.AddHeader(KnownHeaders.Authorization, $"Bearer {accessToken}");
            }
        }
    }
}
