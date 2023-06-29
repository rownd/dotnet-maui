﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rownd.Xamarin.Models.Repos;

namespace Rownd.Xamarin.Core
{
    public class Config
    {
        [JsonProperty("appKey")]
        public string AppKey { get; set; }
        public string HubUrl { get; set; } = "https://hub.rownd.io";
        public string ApiUrl { get; set; } = "https://api.rownd.io";
        public string PostSignInRedirect { get; set; } = "NATIVE_APP";
        public string AppleIdCallbackUrl { get; set; } = "https://api.rownd.io/hub/auth/apple/callback";
        public string SubdomainExtension { get; set; } = ".rownd.link";
        public long DefaultRequestTimeout { get; set; } = 15000;
        public int DefaultNumApiRetries { get; set; } = 5;

        public Customizations Customizations = new Customizations();

        public static Config GetConfig()
        {
            return Shared.ServiceProvider.GetService<Config>();
        }

        public Config()
        {
        }

        public async Task<string> GetHubLoaderUrl()
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var jsonConfig = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            });
            var base64Config = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonConfig), Base64FormattingOptions.None);

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("config", base64Config);

            var stateRepo = StateRepo.Get();

            try
            {
                var signInState = stateRepo.Store.State.SignIn;
                var signInInitStr = signInState.ToSignInInitHash();
                queryString.Add("sign_in", signInInitStr);
            } catch (Exception error)
            {
                Console.WriteLine($"Couldn't compute last sign-in info: {error.Message}");
            }

            UriBuilder uriBuilder = new UriBuilder($"{HubUrl}/mobile_app")
            {
                Query = queryString.ToString()
            };

            try
            {
                var authState = stateRepo.Store.State.Auth;
                var rphInitStr = authState.ToRphInitHash();
                uriBuilder.Fragment = $"rph_init={rphInitStr}";
            }
            catch (Exception error)
            {
                Console.WriteLine($"Couldn't compute requested init hash: {error.Message}");
            }

            Console.WriteLine($"Hub config: {uriBuilder}");

            return uriBuilder.ToString();
        }
    }
}